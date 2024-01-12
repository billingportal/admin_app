namespace BillingPortalClient.Models
{
  public partial class Ticket
  {
    public int Id { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? TicketDate { get; set; }

    public int? CustomerId { get; set; }

    public int? InvoiceId { get; set; }

    public string? InstumentType { get; set; }

    public string? IssueType { get; set; }

    public int? InstrumentNumber { get; set; }

    public int? AccountId { get; set; }

    public string? Image { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Invoice? Invoice { get; set; }

    public virtual ICollection<TicketComment> TicketComments { get; set; } = new List<TicketComment>();
  }
}
