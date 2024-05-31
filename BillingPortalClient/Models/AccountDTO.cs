namespace BillingPortalClient.Models
{
  public class AccountDTO
  {
  }

  public class ChangeAccountDTO
  {
    public int customerId { get; set; }
    public string accountName { get; set; }
    public string accountNumber { get; set; }
    public int accountId { get; set; }
    public string currentUrl { get; set; }
  }
}
