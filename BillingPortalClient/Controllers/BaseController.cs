using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingPortalClient.Controllers
{
  public class BaseController : Controller
  {


    protected string baseUrl = "https://billingportalapis.azurewebsites.net/";
    Uri baseAddress = new Uri( "https://billingportalapis.azurewebsites.net/api/" );
   //Uri baseAddress = new Uri( "http://localhost:7069/api/" );
    Uri oracleBaseAddress = new Uri( "http://88.85.242.29/api/" );

    protected readonly HttpClient _httpClient;
    protected readonly HttpClient _httpClientOracle;
    public BaseController()
    {
      _httpClient = new HttpClient();
      _httpClient.BaseAddress = baseAddress;
      _httpClient.Timeout = TimeSpan.FromMinutes( 25 );

      _httpClientOracle = new HttpClient();
      _httpClientOracle.BaseAddress = oracleBaseAddress;
      _httpClientOracle.Timeout = TimeSpan.FromMinutes( 25 );

    }
  }
}
