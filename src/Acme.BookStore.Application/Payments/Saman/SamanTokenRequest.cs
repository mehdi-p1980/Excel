namespace Acme.BookStore.Payments.Saman
{
    public class SamanTokenRequest
    {
        public string Action { get; set; }
        public string TerminalId { get; set; }
        public long Amount { get; set; }
        public string RedirectUrl { get; set; }
        public string CellNumber { get; set; }
        public string ResNum { get; set; }
    }
}
