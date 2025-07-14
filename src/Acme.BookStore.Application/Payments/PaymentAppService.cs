using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Payments
{
    public class PaymentAppService : ApplicationService, IPaymentAppService
    {
        public Task<string> GetPaymentUrl(Guid planId)
        {
            // TODO: Implement Saman Bank payment gateway integration
            throw new NotImplementedException();
        }

        public Task<bool> VerifyPayment(string token)
        {
            // TODO: Implement Saman Bank payment gateway integration
            throw new NotImplementedException();
        }
    }
}
