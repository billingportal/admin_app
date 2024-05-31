namespace BillingPortalClient.Models
{
  public class RefreshStatementDTO
  {
    public string accountNumber { get; set; }
    public List<Statement> statements { get; set; }
  }
}
