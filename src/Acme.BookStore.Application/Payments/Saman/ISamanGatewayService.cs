using System.Threading.Tasks;

namespace Acme.BookStore.Payments.Saman
{
    public interface ISamanGatewayService
    {
        Task<string> GetToken(long amount, string redirectUrl, string cellNumber = null);
        Task<SamanVerificationResponse> VerifyTransaction(string refNum, string terminalId);
    }
}
