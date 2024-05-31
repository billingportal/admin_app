namespace BillingPortalClient.Models
{
  public class CustomerUser
  {
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public int? CustomerId { get; set; }

    public int? ParentAccountId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Customer? Parent { get; set; }

    public virtual Account? ParentAccount { get; set; }
  }
}
