using Microsoft.AspNetCore.Mvc;

namespace BillingPortalClient.Controllers
{
  public class DashboardController : Controller
  {
    public async Task<ActionResult> Index()
    {
      DashboardModelView dashboardModelView = new DashboardModelView();
      return View(dashboardModelView);
    }
  }
}
