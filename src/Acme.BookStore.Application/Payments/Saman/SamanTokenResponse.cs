namespace Acme.BookStore.Payments.Saman
{
    public class SamanTokenResponse
    {
        public string Token { get; set; }
        public int Status { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDesc { get; set; }
    }
}
