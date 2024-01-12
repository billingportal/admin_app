namespace BillingPortalClient.ModelViews
{
  public class InvoiceRow
  {
    public int id { get; set; }
    public string docNumber { get; set; }
    public DateTime invoiceDate { get; set; }
    public DateTime dueDate { get; set; }
    public string status { get; set; }
    public double? total { get; set; }
    public double? paid { get; set; }
    public double? balance { get; set; }
    public string accountNumber { get; set; }
    public string region { get; set; }
  }
}
