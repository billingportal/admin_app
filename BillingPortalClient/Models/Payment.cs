namespace BillingPortalClient.Models
{
  public partial class Payment
  {
    public int Id { get; set; }

    public int? InvoiceId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal? Amount { get; set; }

    public string? Status { get; set; }

    public string? PaymentMethod { get; set; }

    public int? CustomerId { get; set; }

    public int? AccountId { get; set; }

    public string? AccountNumber { get; set; }

    public string? AccountName { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Invoice? Invoice { get; set; }

    public virtual ICollection<InvoicesPayment> InvoicesPayments { get; set; } = new List<InvoicesPayment>();
  }
}
