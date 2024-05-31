namespace BillingPortalClient.Models
{
  public class InvoicesPayment
  {
    public int Id { get; set; }

    public int? PaymentId { get; set; }

    public int? InvoiceId { get; set; }

    public int? AmountPaid { get; set; }

    public virtual Invoice? Invoice { get; set; }

    public virtual Payment? Payment { get; set; }
  }
}
