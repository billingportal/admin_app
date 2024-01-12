namespace BillingPortalClient.Models
{
  public class RefreshInvoiceDTO
  {
    public string accountNumber { get; set; }
    public List<Invoice> invoices { get; set; }
  }
}
