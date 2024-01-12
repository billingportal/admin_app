namespace BillingPortalClient.ModelViews
{
  public class CustomerViewModel
  {
    public List<BillingSystem.Service.Customer> Customers { get; set; }
    public List<CustomerRow> CustomerRows { get; set; }
  }

  public class CustomerRow
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string AccountNumber { get; set; }
    public string Region { get; set; }
    public string Status { get; set; }
    public bool IsMain { get; set; }
  }
}
