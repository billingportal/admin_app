namespace BillingPortalClient.Models
{
  public partial class Account
  {
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public string? AccountNumber { get; set; }

    public string? AccountName { get; set; }

    public string? ArabicName { get; set; }

    public string? NewAccountNumber { get; set; }

    public bool? IsSelected { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<CustomerUser> CustomerUsers { get; set; } = new List<CustomerUser>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Statement> Statements { get; set; } = new List<Statement>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
  }
}
