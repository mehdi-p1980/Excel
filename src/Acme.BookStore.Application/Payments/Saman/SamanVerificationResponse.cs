namespace Acme.BookStore.Payments.Saman
{
    public class SamanVerificationResponse
    {
        public int ResultCode { get; set; }
        public SamanTransactionDetail TransactionDetail { get; set; }
    }

    public class SamanTransactionDetail
    {
        public string RefNum { get; set; }
        public string TerminalNumber { get; set; }
        public long AffectiveAmount { get; set; }
        public string Rrn { get; set; }
    }
}
