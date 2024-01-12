namespace BillingPortalClient.Models
{
  public class InvoiceStatus
  {
    public int Id { get; set; }

    public string? Status { get; set; }

    public int? InvoiceId { get; set; }

    public virtual Invoice? Invoice { get; set; }
  }
}
