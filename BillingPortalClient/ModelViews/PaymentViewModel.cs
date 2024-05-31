namespace BillingPortalClient.ModelViews
{
  public class PaymentViewModel
  {
    public List<string> selectedInvoices { get; set; }
    public decimal paymentAmount { get; set; }
    public string paymentTransactionId { get; set; }
    public string paymentMethod { get; set; }
     public string accountName { get; set; }
    public string accountNumber { get; set; }
    public List<PaymentRow> paymentRows { get; set; }
    public TicketViewModel ticketViewModel { get; set; }

    public int allReceiptsCount { get; set; }
     public decimal sumReceiptsCount { get; set; }
    public int paidAmountReceiptsTotal { get; set; }
    public int receivedAmountReceiptsTotal { get; set; }
     public string? businessUnitId { get; set; }
   
    //public string accountNumber { get; set; }

  }

  public class PaymentRow
  {
    public int id { get; set; }
    public string accountName { get; set; }
    public string receiverBank { get; set; }
    public string paymentMode { get; set; }
    public DateTime paymentDate { get; set; }
    public decimal paymentAmount { get; set; }
    public string paymentRef { get; set; }
  }
}
