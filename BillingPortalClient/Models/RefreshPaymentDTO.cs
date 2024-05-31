namespace BillingPortalClient.Models
{
  public class RefreshPaymentDTO
  {
    public string accountNumber { get; set; }
    public List<Payment> payments { get; set; }
  }
}
