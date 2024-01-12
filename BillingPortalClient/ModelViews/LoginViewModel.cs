using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BillingPortalClient.ModelViews
{
  public class LoginViewModel
  {
    [Required]
    public string email { get; set; }
    [Required]
    public string password { get; set; }
  }
}
