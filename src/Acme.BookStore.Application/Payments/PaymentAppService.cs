using System;
using System.Threading.Tasks;
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

        public PaymentAppService(
            ISamanGatewayService samanGatewayService,
            IPlanAppService planAppService,
            IHttpContextAccessor httpContextAccessor,
            IMembershipAppService membershipAppService,
            IRepository<Payment, Guid> paymentRepository)
        {
            _samanGatewayService = samanGatewayService;
            _planAppService = planAppService;
            _httpContextAccessor = httpContextAccessor;
            _membershipAppService = membershipAppService;
            _paymentRepository = paymentRepository;
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
                    return true;
                }
            }

            return false;
        }
    }
}
