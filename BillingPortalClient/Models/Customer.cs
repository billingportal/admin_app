namespace BillingPortalClient.Models
{
  public partial class Customer
  {
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Location { get; set; }

    public string? Password { get; set; }

    public string? Designation { get; set; }

    public string? Status { get; set; }

    public bool? IsMain { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<CustomerUser> CustomerUserCustomers { get; set; } = new List<CustomerUser>();

    public virtual ICollection<CustomerUser> CustomerUserParents { get; set; } = new List<CustomerUser>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Statement> Statements { get; set; } = new List<Statement>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
  }
}
