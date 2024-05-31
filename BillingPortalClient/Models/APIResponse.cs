namespace BillingPortalClient.Models
{
  public partial class ApiResponse
{
    public string result { get; set; }
    public string message { get; set; }
    public List<Account> data { get; set; }
}
}