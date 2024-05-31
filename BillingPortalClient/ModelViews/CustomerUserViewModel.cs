using BillingPortalClient.Models;

namespace BillingPortalClient.ModelViews
{
  public class CustomerUserViewModel
  {
    public List<Customer> customerUsers { get; set; }
    public Customer customer { get; set; }
    public CustomerUserDTO customerUserDTO { get; set; }

  }

  public class CustomerUserDTO
  {
    public int id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string password { get; set; }
    public string accountName { get; set; }
    public string accountNumber { get; set; }
    public string designation { get; set; }
    public bool isActive { get; set; }
  }
}
