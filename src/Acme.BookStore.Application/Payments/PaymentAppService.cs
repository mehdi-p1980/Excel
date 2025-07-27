using System;
using System.Threading.Tasks;
using Acme.BookStore.Accounting;
using Acme.BookStore.Memberships;
using Acme.BookStore.Payments.Saman;
using Acme.BookStore.Plans;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Payments
{
    public class PaymentAppService : ApplicationService, IPaymentAppService
    {
        private readonly ISamanGatewayService _samanGatewayService;
        private readonly IPlanAppService _planAppService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMembershipAppService _membershipAppService;
        private readonly IRepository<Payment, Guid> _paymentRepository;
        private readonly IAccountingAppService _accountingAppService;
        private readonly IRepository<Account, Guid> _accountRepository;

        public PaymentAppService(
            ISamanGatewayService samanGatewayService,
            IPlanAppService planAppService,
            IHttpContextAccessor httpContextAccessor,
            IMembershipAppService membershipAppService,
            IRepository<Payment, Guid> paymentRepository,
            IAccountingAppService accountingAppService,
            IRepository<Account, Guid> accountRepository)
        {
            _samanGatewayService = samanGatewayService;
            _planAppService = planAppService;
            _httpContextAccessor = httpContextAccessor;
            _membershipAppService = membershipAppService;
            _paymentRepository = paymentRepository;
            _accountingAppService = accountingAppService;
            _accountRepository = accountRepository;
        }

        public async Task<string> GetPaymentUrl(Guid planId)
        {
            var plan = await _planAppService.GetAsync(planId);
            var redirectUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/payment-verification";
            var token = await _samanGatewayService.GetToken((long)plan.Price, redirectUrl);

            return $"https://sep.shaparak.ir/payment.aspx?token={token}";
        }

        public async Task<bool> VerifyPayment(string token)
        {
            var refNum = _httpContextAccessor.HttpContext.Request.Query["RefNum"];
            var terminalId = _httpContextAccessor.HttpContext.Request.Query["TerminalId"];
            var verificationResponse = await _samanGatewayService.VerifyTransaction(refNum, terminalId);

            if (verificationResponse.ResultCode == 0)
            {
                var payment = await _paymentRepository.FirstOrDefaultAsync(p => p.TransactionId == refNum);
                if (payment != null)
                {
                    payment.Status = PaymentStatus.Completed;
                    await _paymentRepository.UpdateAsync(payment);
                    await _membershipAppService.ExtendMembership(payment.PlanId);

                    var plan = await _planAppService.GetAsync(payment.PlanId);
                    var cashAccount = await _accountRepository.FirstOrDefaultAsync(a => a.Name == "Cash");
                    var salesRevenueAccount = await _accountRepository.FirstOrDefaultAsync(a => a.Name == "Sales Revenue");

                    if (cashAccount != null && salesRevenueAccount != null)
                    {
                        var lines = new[]
                        {
                            new JournalEntryLineDto { AccountId = cashAccount.Id, Debit = plan.Price, Credit = 0 },
                            new JournalEntryLineDto { AccountId = salesRevenueAccount.Id, Debit = 0, Credit = plan.Price }
                        };
                        await _accountingAppService.CreateJournalEntryAsync(DateTime.UtcNow, $"Payment for plan {plan.Name}", lines);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
