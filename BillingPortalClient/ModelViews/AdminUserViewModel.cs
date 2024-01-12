using BillingSystem.Service;

namespace BillingPortalClient.ModelViews
{
  public class AdminUserViewModel
  {
    public List<Admin> adminUsers { get; set; }
    public Admin adminUser { get; set; }
    public AdminPermissionsViewModel adminPermission { get; set; }
    public List<Region> regions { get; set; }
    public List<int> selectedRegions { get; set; }
       // public string SelectedRegion { get; set; }
    }

  public class AdminPermissionsViewModel
  {
    public int Id { get; set; }
    public int AdminId { get; set; }
    public bool ViewInvoice { get; set; }
    public bool UpdateInvoice { get; set; }
    public bool ViewPayment { get; set; }
    public bool UpdatePayment { get; set; }
    public bool ViewStatement { get; set; }
    public bool UpdateStatement { get; set; }
    public bool ViewTicket { get; set; }
    public bool UpdateTicket { get; set; }
    public bool ViewCustomer { get; set; }
    public bool UpdateCustomer { get; set; }
  }
}
