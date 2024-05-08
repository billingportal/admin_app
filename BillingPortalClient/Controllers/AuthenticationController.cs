using Microsoft.AspNetCore.Mvc;
using BillingPortalClient.ModelViews;
using System.Diagnostics;
using BillingPortalClient.Models;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using BillingPortalClient.Components;
using Microsoft.AspNetCore.Authorization;

namespace BillingPortalClient.Controllers
{
  public class AuthenticationController : BaseController
  {


        private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController( ILogger<AuthenticationController> logger )
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
    public async Task<ActionResult> Login(LoginViewModel loginViewModel )
    {
      if(!ModelState.IsValid)
      {
        return View(loginViewModel);
      }
      BillingSystem.Service.Admin loginResult = new BillingSystem.Service.Admin();
      //Customer loginResult;
      using( var response = await _httpClient.GetAsync( $"Authentication/loginAdmin/{loginViewModel.email}/{loginViewModel.password}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        loginResult = JsonConvert.DeserializeObject<BillingSystem.Service.Admin>( apiResponse );
      }
      if( loginResult != null )
      {
        if(loginResult.Status == "Inactive")
        {
          ViewBag.loginMessage = "User account is inactive.";
          return View();
        }
        if(loginResult.Status == "Suspended")
        {
          ViewBag.loginMessage = "User account is suspended.";
          return View();
        }

        List<Claim> claims = new List<Claim>();
        claims.Add( new Claim( "adminId", loginResult.Id.ToString() ) );
        claims.Add( new Claim( "adminRole", loginResult.Role ) );
        claims.Add( new Claim( "adminFirstName", loginResult.FirstName ) );
        claims.Add( new Claim( "adminLastName", loginResult.LastName ) );
        claims.Add( new Claim( "adminEmail", loginResult.Email ) );
        claims.Add( new Claim( "adminStatus", loginResult.Status ) );
        if(loginResult.AdminPermissions.Count > 0)
        {
          claims.Add( new Claim( "viewInvoice", loginResult.AdminPermissions.FirstOrDefault().ViewInvoice.ToString(), ClaimValueTypes.Boolean ) );
          claims.Add( new Claim( "updateInvoice", loginResult.AdminPermissions.FirstOrDefault().UpdateInvoice.ToString(), ClaimValueTypes.Boolean ) );
          claims.Add( new Claim( "viewPayment", loginResult.AdminPermissions.FirstOrDefault().ViewPayment.ToString(), ClaimValueTypes.Boolean ) );
          claims.Add( new Claim( "updatePayment", loginResult.AdminPermissions.FirstOrDefault().UpdatePayment.ToString(), ClaimValueTypes.Boolean ) );
          claims.Add( new Claim( "viewStatement", loginResult.AdminPermissions.FirstOrDefault().ViewStatement.ToString(), ClaimValueTypes.Boolean ) );
          claims.Add( new Claim( "updateStatement", loginResult.AdminPermissions.FirstOrDefault().UpdateStatement.ToString(), ClaimValueTypes.Boolean ) );
          claims.Add( new Claim( "viewCustomer", loginResult.AdminPermissions.FirstOrDefault().ViewCustomer.ToString(), ClaimValueTypes.Boolean ) );
          claims.Add( new Claim( "updateCustomer", loginResult.AdminPermissions.FirstOrDefault().UpdateCustomer.ToString(), ClaimValueTypes.Boolean ) );
          claims.Add( new Claim( "viewTicket", loginResult.AdminPermissions.FirstOrDefault().ViewTicket.ToString(), ClaimValueTypes.Boolean ) );
          claims.Add( new Claim( "updateTicket", loginResult.AdminPermissions.FirstOrDefault().UpdateTicket.ToString(), ClaimValueTypes.Boolean ) );
        }



        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
          CookieAuthenticationDefaults.AuthenticationScheme);

        AuthenticationProperties properties = new AuthenticationProperties()
        {
          AllowRefresh = true,
          IsPersistent = true
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
          new ClaimsPrincipal(claimsIdentity), properties);

       
        return RedirectToAction( "index", "Home" );
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


  }
}
