namespace Acme.BookStore.Payments.Saman
{
    public class SamanGatewayOptions
    {
        public string TerminalId { get; set; }
        public string ApiTokenUrl { get; set; }
        public string ApiVerificationUrl { get; set; }
        public string PaymentPageUrl { get; set; }
    }
}
