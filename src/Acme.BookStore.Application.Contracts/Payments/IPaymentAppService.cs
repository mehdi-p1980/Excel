using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Payments
{
    public interface IPaymentAppService : IApplicationService
    {
        Task<string> GetPaymentUrl(Guid planId);
        Task<bool> VerifyPayment(string token);
    }
}
