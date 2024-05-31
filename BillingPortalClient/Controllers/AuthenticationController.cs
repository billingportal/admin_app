using System.Text;
using System.Net.Http;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using BillingPortalClient.ModelViews;
using BillingPortalClient.Models;

namespace BillingPortalClient.Controllers
{
    public class AuthenticationController : BaseController
    {
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Login()
        {
          Console.WriteLine( "Hiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii!" );
          return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            BillingSystem.Service.Admin loginResult = null;

            try
            {
                string requestUri = new Uri(baseAddress, $"Authentication/loginAdmin/{loginViewModel.email}/{loginViewModel.password}").ToString();
                using (var response = await _httpClient.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    loginResult = JsonConvert.DeserializeObject<BillingSystem.Service.Admin>(apiResponse);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while calling the login API.");
                ViewBag.loginMessage = "There was an error contacting the login service.";
                return View(loginViewModel);
            }

            if (loginResult != null)
            {
                if (loginResult.Status == "Inactive")
                {
                    ViewBag.loginMessage = "User account is inactive.";
                    return View();
                }
                if (loginResult.Status == "Suspended")
                {
                    ViewBag.loginMessage = "User account is suspended.";
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim("adminId", loginResult.Id.ToString()),
                    new Claim("adminRole", loginResult.Role),
                    new Claim("adminFirstName", loginResult.FirstName),
                    new Claim("adminLastName", loginResult.LastName),
                    new Claim("adminEmail", loginResult.Email),
                    new Claim("adminStatus", loginResult.Status)
                };

                if (loginResult.AdminPermissions.Count > 0)
                {
                    var permissions = loginResult.AdminPermissions.FirstOrDefault();
                    claims.Add(new Claim("viewInvoice", permissions.ViewInvoice.ToString(), ClaimValueTypes.Boolean));
                    claims.Add(new Claim("updateInvoice", permissions.UpdateInvoice.ToString(), ClaimValueTypes.Boolean));
                    claims.Add(new Claim("viewPayment", permissions.ViewPayment.ToString(), ClaimValueTypes.Boolean));
                    claims.Add(new Claim("updatePayment", permissions.UpdatePayment.ToString(), ClaimValueTypes.Boolean));
                    claims.Add(new Claim("viewStatement", permissions.ViewStatement.ToString(), ClaimValueTypes.Boolean));
                    claims.Add(new Claim("updateStatement", permissions.UpdateStatement.ToString(), ClaimValueTypes.Boolean));
                    claims.Add(new Claim("viewCustomer", permissions.ViewCustomer.ToString(), ClaimValueTypes.Boolean));
                    claims.Add(new Claim("updateCustomer", permissions.UpdateCustomer.ToString(), ClaimValueTypes.Boolean));
                    claims.Add(new Claim("viewTicket", permissions.ViewTicket.ToString(), ClaimValueTypes.Boolean));
                    claims.Add(new Claim("updateTicket", permissions.UpdateTicket.ToString(), ClaimValueTypes.Boolean));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var properties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

                return RedirectToAction("index", "Home");
            }
            else
            {
                ViewBag.loginMessage = "Email or Password is not correct.";
                return View();
            }
        }

    public async Task<ActionResult> OTP(string email)
    {
      ViewBag.emailForOTP = email;
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> OTP(OTP otp)
    {
      string otpCode = otp.digit1.ToString() + otp.digit2.ToString() + otp.digit3.ToString() + otp.digit4.ToString();
      string otpVerifyResult;
      using( var response = await _httpClient.GetAsync( $"Authentication/VerifyOTP/{otp.email}/{otpCode}" ) )
      {
        otpVerifyResult = await response.Content.ReadAsStringAsync();
        //otpVerifyResult = JsonConvert.DeserializeObject<string>( apiResponse );
      }
      if(otpVerifyResult == "OTP code has been verified." )
      {
        return RedirectToAction("ResetPassword", new { otp.email } );
      }
      ViewBag.emailForOTP = otp.email;
      ViewBag.otpResult = otpVerifyResult;
      return View();
    }

    public async Task<ActionResult> ResetPassword(string email)
    {
      ViewBag.emailForResetPassword = email;
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
    {
      ViewBag.emailForResetPassword = resetPasswordViewModel.email;
      if(resetPasswordViewModel.password != resetPasswordViewModel.confirmPassword)
      {
        ViewBag.resetPasswordMessage = "Passwords do not match.";
        return View();
      }

      bool resetPassword;
      using( var response = await _httpClient.GetAsync( $"Authentication/ResetPassword/{resetPasswordViewModel.email}/{resetPasswordViewModel.password}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        resetPassword = JsonConvert.DeserializeObject<bool>( apiResponse );
      }
      if( !resetPassword )
      {
        return View();
      }

      return RedirectToAction( "Login" );

    }



    public async Task<ActionResult> OTPRegister(string email)
    {
      ViewBag.emailForOTP = email;
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> OTPRegister(OTP otp)
    {
      string otpCode = otp.digit1.ToString() + otp.digit2.ToString() + otp.digit3.ToString() + otp.digit4.ToString();
      string otpVerifyResult;
      using( var response = await _httpClient.GetAsync( $"Authentication/VerifyOTP/{otp.email}/{otpCode}" ) )
      {
        otpVerifyResult = await response.Content.ReadAsStringAsync();
        //otpVerifyResult = JsonConvert.DeserializeObject<string>( apiResponse );
      }
      if( otpVerifyResult == "OTP code has been verified." )
      {
        return RedirectToAction( "RegisterProfile" );
      }
      ViewBag.emailForOTP = otp.email;
      ViewBag.otpResult = otpVerifyResult;
      return View();
    }

    

    public async Task<ActionResult> ForgotPassword()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
    {
      bool result;
      using( var response = await _httpClient.GetAsync( $"Authentication/ForgetPasswordAdmin/{forgotPasswordViewModel.email}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        result = JsonConvert.DeserializeObject<bool>( apiResponse );
      }
      if( result )
      {
        return RedirectToAction( "OTP", new { forgotPasswordViewModel.email } );
      }
      else
      {
        ViewBag.forgotPasswordMessage = "User does not exist with this email address.";
        return View();
      }
    }

    public async Task<ActionResult> LoggedIn()
    {
      ClaimsPrincipal claimsUser = HttpContext.User;
      return View();
    }

    [Authorize]
    public async Task<ActionResult> Logout()
    {
      await HttpContext.SignOutAsync( CookieAuthenticationDefaults.AuthenticationScheme );
      return RedirectToAction( "Login", "Authentication" );
    }

    public async Task<ActionResult<List<Account>>> GetCurrentCustomerAccounts()
    {
      int customerId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault().Value );
      List<Account> accounts = new List<Account>();
      using( var response = await _httpClient.GetAsync( $"Customer/GetAccountsLinked/{customerId}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        if(apiResponse != null)
        {
          accounts = JsonConvert.DeserializeObject<List<Account>>( apiResponse );
        }
      }

      return accounts;
    }

    public async Task<ActionResult<Admin>> GetCurrentAdmin()
    {
      int adminId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault().Value );
      Admin admin = new Admin();
      using( var response = await _httpClient.GetAsync( $"Admin/GetAdminProfile/{adminId}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        if( apiResponse != null )
        {
          if( JsonConvert.DeserializeObject<Admin>( apiResponse ) != null)
          {
            admin = JsonConvert.DeserializeObject<Admin>( apiResponse );
          }
        }
      }

      return admin;
    }


    [HttpPost]
    public async Task<IActionResult> UpdateCustomerClaims([FromBody] Suggestion suggestion)
    {
        if (suggestion == null)
        {
            return BadRequest("Invalid suggestion data.");
        }

        Customer customer = null;
            string requestUri = new Uri(baseAddress, "Customer/GetCustomerByAccountNumber/" + suggestion.accountNumber).ToString();
            using (var response = await _httpClient.GetAsync(requestUri))
              {
                string apiResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"api response: {apiResponse}");
                customer = JsonConvert.DeserializeObject<Customer>(apiResponse);    
                   
            }

          
        if (customer != null)
            {
            var customerId = customer.Id.ToString(); 
            var account = customer.Accounts.FirstOrDefault(); // Get the first account or handle accordingly

            if (account != null)
            {
                var accountId = account.Id.ToString();
                 Console.WriteLine($"customer id: {customerId}");

                 // Create a new list to hold the merged claims
                var mergedClaims = new List<Claim>();

                // Add existing claims to the merged list
                mergedClaims.AddRange(User.Claims);

                // Define the claim types to be removed
                var claimTypesToRemove = new[]
                {
                    "custCustomerId",
                    "custAccountId",
                    "custAccountName",
                    "custArabicName",
                    "custAccountNumber",
                    "custNewAccountNumber",
                    "custEmail",
                    "custCity",
                    "custCourierRoute",
                    "custRegion",
                    "custBusinessUnitId"
                };

                // Remove existing claims with the specified types
                mergedClaims = mergedClaims.Where(claim => !claimTypesToRemove.Contains(claim.Type)).ToList();


                // Add new claims from the method
                mergedClaims.AddRange(new[]
                {
                    new Claim("custCustomerId", customerId ?? string.Empty),
                    new Claim("custAccountId", accountId ?? string.Empty),
                    new Claim("custAccountName", suggestion.accountName ?? string.Empty),
                    new Claim("custArabicName", suggestion.arabicName ?? string.Empty),
                    new Claim("custAccountNumber", suggestion.accountNumber ?? string.Empty),
                    new Claim("custNewAccountNumber", suggestion.accountNumber ?? string.Empty),
                    new Claim("custEmail", suggestion.email ?? string.Empty),
                    new Claim("custCity", suggestion.city ?? string.Empty),
                    new Claim("custCourierRoute", suggestion.courierRoute ?? string.Empty),
                    new Claim("custRegion", suggestion.region ?? string.Empty),
                    new Claim("custBusinessUnitId", suggestion.businessUnitId ?? string.Empty)
                });

                // Create the claims identity with the merged claims
                var claimsIdentity = new ClaimsIdentity(mergedClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                var properties = new AuthenticationProperties
                {
                            AllowRefresh = true,
                            IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
           
            } else {
                // Handle the case where no accounts are available
                Console.WriteLine("No accounts found for the customer.");
            }

        } else {
                // Handle the case where no customers are returned
                Console.WriteLine("No customers found for the given account number.");
        }

        // Redirect to Invoice/Index after updating claims
         return RedirectToAction("Index", "Invoice");
    }

    


  }
}
