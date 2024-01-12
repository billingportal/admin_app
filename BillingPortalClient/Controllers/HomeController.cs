using System.Diagnostics;
//using BillingPortalClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Newtonsoft.Json;
using System.Text;
using BillingPortalClient.Models;
using BillingSystem.Service;

namespace BillingPortalClient.Controllers
{
  public class HomeController : BaseController
  {

    //Uri baseAddress = new Uri( "https://localhost:7069/api/" );
    //Uri baseAddress = new Uri( "https://billingportalapis.azurewebsites.net/" );

    //private readonly HttpClient _httpClient;

    private readonly ILogger<HomeController> _logger;


    public HomeController( ILogger<HomeController> logger )
    {
      _logger = logger;
      //_httpClient = new HttpClient();
      //_httpClient.BaseAddress = baseAddress;
    }

    public async Task<ActionResult> Index()
    {
      using( var response = await _httpClient.GetAsync($"Admin/GetAdminProfile/{1}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        //reservationList = JsonConvert.DeserializeObject<List<Reservation>>( apiResponse );
      }
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
    public IActionResult Error()
    {
      return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
    }

    [HttpPost]
    public async Task<ActionResult> SetLanguage(string culture, string returnUrl)
    {
      Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue( new RequestCulture( culture ) ),
        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears( 1 ) }
        );
      return LocalRedirect( returnUrl );
    }

    public async Task<ActionResult<string>> GetAmazonPage()
    {
      BillingSystem.Service.PaymentRequestView paymentRequestView = new BillingSystem.Service.PaymentRequestView();
      paymentRequestView.ResponseArgs = new Dictionary<string, string>()
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };
      paymentRequestView.CustArgs = new Dictionary<string, string>()
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };
      paymentRequestView.MerchantId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");
      paymentRequestView.GatewayOption = "PAYFORT";
      paymentRequestView.GatewayId = 0;
      paymentRequestView.Language = "en";
      paymentRequestView.PaymentType = "string";
      paymentRequestView.UserAgent = "string";
      paymentRequestView.CurrencyCode = "AED";
      paymentRequestView.Amount = 11;
      paymentRequestView.OrderId = 2;
      paymentRequestView.Channel = "string";
      paymentRequestView.Email = "test1@abc.com";
      paymentRequestView.Mobile = "string";
      paymentRequestView.CustomerName = "string";
      paymentRequestView.Token = "string1";
      //paymentRequestView.ReturnURL = "https://billingportalwebapp.azurewebsites.net/";
      paymentRequestView.ReturnURL = "https://55b9-139-135-59-47.ngrok-free.app/Home/CompeletePaymentAmazon";
      paymentRequestView.Mode = "string";
      paymentRequestView.PaymentOption = "string";
      paymentRequestView.CardNumber = "string";
      paymentRequestView.Description = "string";
      paymentRequestView.UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");
      paymentRequestView.Otp = "string";



      var json = JsonConvert.SerializeObject( paymentRequestView );
      var content = new StringContent( json, Encoding.UTF8, "application/json" );

      BillingSystem.Service.PaymentResponseView paymentResponseView = new BillingSystem.Service.PaymentResponseView();
      using( var response = await _httpClient.PostAsync( $"PaymentGateWay/IntializePayment/", content ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        if( apiResponse != null )
        {
          try
          {
            paymentResponseView = JsonConvert.DeserializeObject<BillingSystem.Service.PaymentResponseView>( apiResponse );

          }
          catch( Exception ex )
          {
            throw new Exception(ex.Message );
          }
        }
      }

      //string htmlContent = "<html><head><script src='https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js'></script><script type='text/javascript'>$(window).on('load', function(){debugger;$(\"form[name='Form1']\").submit();});</script></head><body onload='document.Form1.submit()'><form name='Form1' method='post' action='https://sbcheckout.payfort.com/FortAPI/paymentPage' ><input name='signature' type='hidden' value='469cdc36de1040eb83d49a198958627ed9498521c246d9d5a56e538a1079a3c1'><input name='command' type='hidden' value='AUTHORIZATION'><input name='merchant_reference' type='hidden' value='mbn-mirza-gmail-com-SB'><input name='amount' type='hidden' value=10'><input name='access_code' type='hidden' value='I8pATsIo8ddo0j4WnPk6'><input name='merchant_identifier' type='hidden' value='XvsRZoyu'><input name='currency' type='hidden' value='SAR'><input name='language' type='hidden' value='en'><input name='customer_email' type='hidden' value='abc@test.com'><input name='customer_name' type='hidden' value='testCust'><input name='token_name' type='hidden' value='string'><input name='return_url' type='hidden' value='string?GatewayId=0&GatewayOption=PAYFORT&OrderId=0'></form></body></html>";



      return paymentResponseView.PostData;
    }

    [HttpPost]
    public async Task<ActionResult> CompeletePaymentAmazon(PaymentResponseViewDTO paymentResponseViewDTO )
    {
      if( paymentResponseViewDTO != null )
      {
        if( paymentResponseViewDTO.response_message == "Success" )
        {
          //string _accountNumber = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "customerAccountNumber" ).Value;
          try
          {
            PaymentDTO paymentDTO = new PaymentDTO();

            paymentDTO.PaymentAmount = Convert.ToDouble( paymentResponseViewDTO.amount/100);
            //paymentDTO.PaymentAmount = 1;
            string myString = paymentResponseViewDTO.merchant_reference;
            char[] separators = { '_' };
            string[] parts = myString.Split( separators );

            paymentDTO.AccountNumber = parts[ 0 ];

            var client = new Client( baseUrl, _httpClient );
            List<MerchantInvoiceReference> merchantInvoiceReferences = ( await client.GetMerchantReferenceInvoicesAsync( paymentResponseViewDTO.merchant_reference ) ).ToList();
            paymentDTO.SelectedInvoices = merchantInvoiceReferences.Select( x => x.InvoiceNumer ).ToList();

            var json = JsonConvert.SerializeObject( paymentDTO );
            var content = new StringContent( json, Encoding.UTF8, "application/json" );

            bool invoicesResult;
            using( var response = await _httpClient.PostAsync( $"Payment/SavePayment/", content ) )
            {
              string apiResponse = await response.Content.ReadAsStringAsync();
              invoicesResult = JsonConvert.DeserializeObject<bool>( apiResponse );
            }

            //bool paymentResult = await new Client( baseUrl, _httpClient ).SavePayment2Async( paymentDTO );
            CustomerContext customerContext = await new Client( baseUrl, _httpClient ).GetCurrentUserContextAsync( paymentResponseViewDTO.merchant_reference );


            return RedirectToAction( "Payments", "Payment" );
          }
          catch ( Exception ex )
          {
            throw new Exception( ex.Message );
          }
        }
      }

      return RedirectToAction( "Index", "Invoice" );
    }
  }
}