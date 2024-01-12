namespace BillingPortalClient.Models
{
  public class InvoiceDTO
  {
    public string transactionClass { get; set; }
    public string docNumber { get; set; }
    public DateTime glDate { get; set; }
    public DateTime trxDate { get; set; }
    public DateTime? dueDate { get; set; }
    public string status { get; set; }
    public double debit { get; set; }
    public double credit { get; set; }
    public int customerPartyId { get; set; }
    public int custTrxTypeId { get; set; }
    public string refNo { get; set; }
    public string accountNumber { get; set; }
    public string accountName { get; set; }
    public string oldAccountId { get; set; }
  }
}
