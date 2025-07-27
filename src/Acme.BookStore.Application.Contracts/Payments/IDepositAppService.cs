using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Payments
{
    public interface IDepositAppService : IApplicationService
    {
        Task SubmitDepositReceipt(Guid planId, byte[] receipt);
        Task ConfirmDeposit(Guid paymentId);
        Task RejectDeposit(Guid paymentId);
    }
}
