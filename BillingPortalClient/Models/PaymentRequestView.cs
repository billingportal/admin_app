namespace BillingPortalClient.Models
{
    public class PaymentRequestView
    {
        public Guid MerchantId { get; set; }
        public string GatewayOption { get; set; }
        public int GatewayId { get; set; }
        public string language { get; set; }
        public string PaymentType { get; set; }
        public string UserAgent { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public long OrderId { get; set; }
        public string Channel { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string CustomerName { get; set; }
        public string Token { get; set; }
        public string ReturnURL { get; set; }
        public string Mode { get; set; }
        public Dictionary<string, string> ResponseArgs { get; set; }
        public string PaymentOption { get; set; }
        public string CardNumber { get; set; }
        public Dictionary<string, string> custArgs { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public string OTP { get; set; }
    }
}
