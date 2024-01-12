namespace BillingPortalClient.Models
{
    public class PaymentResponseView
    {
        public Guid MerchantId { get; set; }
        public Guid UserId { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string HttpMethod { get; set; }
        public string PostData { get; set; }
        public long TransactionId { get; set; }
        public bool IsSuccess { get; set; }
        public long OrderId { get; set; }
        public string GatewayOption { get; set; }
        public string PaymentType { get; set; }
        public string PGReferenceId { get; set; }
        public decimal PaidAmount { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> PaymentMessage { get; set; }
        public decimal CardBalance { get; internal set; }
        public string TokenNumber { get; internal set; }
        public string OTPRefCode { get; internal set; }
        public long CardNumber { get; internal set; }
        public string PostURI { get; set; }
    }
}
