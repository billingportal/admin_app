namespace BillingPortalClient.Models
{
  public partial class Invoice
  {
    public int Id { get; set; }

    public string? TransactionClass { get; set; }

    public string? DocNumber { get; set; }

    public DateTime? GlDate { get; set; }

    public DateTime? TrxDate { get; set; }

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }

    public int? CustomerPartyId { get; set; }

    public int? CustTrxTypeId { get; set; }

    public string? RefNo { get; set; }

    public string? AccountNumber { get; set; }

    public string? AccountName { get; set; }

    public string? OldAccountId { get; set; }

    public int? AccountId { get; set; }

    public int? CustomerId { get; set; }

    public string? Status { get; set; }

    public DateTime? DueDate { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<InvoiceStatus> InvoiceStatuses { get; set; } = new List<InvoiceStatus>();

    public virtual ICollection<InvoicesPayment> InvoicesPayments { get; set; } = new List<InvoicesPayment>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
  }
}
