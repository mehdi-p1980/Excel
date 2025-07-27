using System;
using System.Threading.Tasks;
using Acme.BookStore.Memberships;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Acme.BookStore.Payments
{
    public class DepositAppService : ApplicationService, IDepositAppService
    {
        private readonly IRepository<Payment, Guid> _paymentRepository;
        private readonly IMembershipAppService _membershipAppService;
        private readonly ICurrentUser _currentUser;

        public DepositAppService(
            IRepository<Payment, Guid> paymentRepository,
            IMembershipAppService membershipAppService,
            ICurrentUser currentUser)
        {
            _paymentRepository = paymentRepository;
            _membershipAppService = membershipAppService;
            _currentUser = currentUser;
        }

        public async Task SubmitDepositReceipt(Guid planId, byte[] receipt)
        {
            var userId = _currentUser.Id.GetValueOrDefault();
            var payment = new Payment
            {
                UserId = userId,
                PlanId = planId,
                PaymentMethod = PaymentMethod.Deposit,
                Status = PaymentStatus.Pending
            };

            await _paymentRepository.InsertAsync(payment);
        }

        public async Task ConfirmDeposit(Guid paymentId)
        {
            var payment = await _paymentRepository.GetAsync(paymentId);
            payment.Status = PaymentStatus.Completed;
            await _paymentRepository.UpdateAsync(payment);

            await _membershipAppService.ExtendMembership(payment.PlanId);
        }

        public async Task RejectDeposit(Guid paymentId)
        {
            var payment = await _paymentRepository.GetAsync(paymentId);
            payment.Status = PaymentStatus.Failed;
            await _paymentRepository.UpdateAsync(payment);
        }
    }
}
