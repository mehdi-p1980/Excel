using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.DependencyInjection;

namespace Acme.BookStore.Payments.Saman
{
    public class SamanGatewayService : ISamanGatewayService, ITransientDependency
    {
        private readonly HttpClient _httpClient;
        private readonly SamanGatewayOptions _options;

        public SamanGatewayService(HttpClient httpClient, IOptions<SamanGatewayOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<string> GetToken(long amount, string redirectUrl, string cellNumber = null)
        {
            var request = new SamanTokenRequest
            {
                Action = "token",
                TerminalId = _options.TerminalId,
                Amount = amount,
                RedirectUrl = redirectUrl,
                CellNumber = cellNumber,
                ResNum = "123"
            };

            var response = await _httpClient.PostAsJsonAsync(_options.ApiTokenUrl, request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<SamanTokenResponse>(responseContent);

            return tokenResponse.Token;
        }

        public async Task<SamanVerificationResponse> VerifyTransaction(string refNum, string terminalId)
        {
            var request = new SamanVerificationRequest
            {
                RefNum = refNum,
                TerminalNumber = terminalId
            };

            var response = await _httpClient.PostAsJsonAsync(_options.ApiVerificationUrl, request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var verificationResponse = JsonConvert.DeserializeObject<SamanVerificationResponse>(responseContent);

            return verificationResponse;
        }
    }
}
