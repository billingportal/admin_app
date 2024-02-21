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
    //Uri baseAddress = new Uri( "https://localhost:7069/api/" );
    //Uri baseAddress = new Uri( "https://billingportalapis.azurewebsites.net/" );

    //Uri oracleBaseAddress = new Uri( "http://88.85.242.29/api/" );
    //private readonly HttpClient _httpClient;
        //private readonly HttpClient _httpClientOracle;

        private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController( ILogger<AuthenticationController> logger )
    {
      _logger = logger;
      //_httpClient = new HttpClient();
      //_httpClient.BaseAddress = baseAddress;
      //_httpClientOracle = new HttpClient();
      //_httpClientOracle.BaseAddress = oracleBaseAddress;
    }
    public IActionResult Index()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult<string>> ChangeCustomerAccount( [FromBody] ChangeAccountDTO changeAccountDTO )
    {
      bool isAccountSaved;
      using( var response = await _httpClient.GetAsync( $"Customer/MarkCustomerAccountSelected/{changeAccountDTO.accountId}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        if( apiResponse != null )
        {
          isAccountSaved = JsonConvert.DeserializeObject<bool>( apiResponse );
        }
      }

      Account account = new Account();
      using( var response = await _httpClient.GetAsync( $"Customer/GetAccountByAccountId/{changeAccountDTO.accountId}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        if( apiResponse != null )
        {
          account = JsonConvert.DeserializeObject<Account>( apiResponse );
        }
      }

      

      var identity = HttpContext.User.Identity as ClaimsIdentity;

      // Check if the user is authenticated and the identity exists
      if( identity != null && identity.IsAuthenticated )
      {
        // Find the claim you want to update
        var existingClaimAccountNumber = identity.FindFirst( "customerAccountNumber" );

        // Remove the existing claim if it exists
        if( existingClaimAccountNumber != null )
        {
          identity.RemoveClaim( existingClaimAccountNumber );
        }

        var existingClaimAccountId = identity.FindFirst( "accountId" );

        // Remove the existing claim if it exists
        if( existingClaimAccountId != null )
        {
          identity.RemoveClaim( existingClaimAccountId );
        }

        var existingClaimAccountName = identity.FindFirst( "accountName" );

        // Remove the existing claim if it exists
        if( existingClaimAccountName != null )
        {
          identity.RemoveClaim( existingClaimAccountName );
        }
        // Add a new or updated claim
        identity.AddClaim( new Claim( "customerAccountNumber", account.AccountNumber ) );
        identity.AddClaim( new Claim( "accountId", account.Id.ToString() ) );
        identity.AddClaim( new Claim( "accountName", account.AccountName ) );
        //ClaimsIdentity claimsIdentity = new ClaimsIdentity( claims,
        //  CookieAuthenticationDefaults.AuthenticationScheme );

        AuthenticationProperties properties = new AuthenticationProperties()
        {
          AllowRefresh = true,
          IsPersistent = true
        };

        await HttpContext.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme,
          new ClaimsPrincipal( identity ), properties );

        // Optionally, you might need to refresh or renew the user's authentication token
        // and reissue it with updated claims. This might involve creating a new token
        // with the updated claims and re-authenticating the user with the new token.
        // This depends on your specific authentication mechanism and requirements.
      }

      return changeAccountDTO.currentUrl;
    }

    public async Task<ActionResult> Login()
    {
      Console.WriteLine( "Hiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii!" );

      //ClaimsPrincipal claimsUser = HttpContext.User;
      //if(claimsUser.Identity.IsAuthenticated)
      //{
      //  return RedirectToAction("LoggedIn");
      //}
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

    public async Task<ActionResult> Register()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Register(RegisterViewModel registerViewModel)
    {
      bool isCustomerExist;
      using( var response = await _httpClient.GetAsync( $"Authentication/IsCustomerAlreadyRegistered/{registerViewModel.email}/{registerViewModel.accountNumber}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        isCustomerExist = JsonConvert.DeserializeObject<bool>( apiResponse );
      }
      if(isCustomerExist)
      {
        ViewBag.responseMessage = "Customer is already registered.";
        return View();
      }

      string result = "";
      using( var response = await _httpClientOracle.GetAsync( $"Authentication/IsUserExistInOracleDatabase/{registerViewModel.email}/{registerViewModel.accountNumber}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        if( apiResponse != null )
        {
          result = apiResponse;
        }
      }
      //if( result != "Exists" )
      //{
      //  ViewBag.responseMessage = result;
      //  return View();
      //}

      bool otpResult;
      using( var response = await _httpClient.GetAsync( $"Authentication/SendOtpForRegistration/{registerViewModel.email}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        otpResult = JsonConvert.DeserializeObject<bool>( apiResponse );
      }

      HttpContext.Session.SetString( "custRegAccountNumber", registerViewModel.accountNumber );
      return RedirectToAction("OTPRegister" , new { registerViewModel.email } );
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

    public async Task<ActionResult> RegisterProfile()
    {
      string custRegAccountNumber = HttpContext.Session.GetString( "custRegAccountNumber" );
      OracleCustomer oracleCustomer = new OracleCustomer();
      using( var response = await _httpClientOracle.GetAsync( $"Customer/GetCustomerFromOracleDatabase/{custRegAccountNumber}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        oracleCustomer = JsonConvert.DeserializeObject<OracleCustomer>( apiResponse );
      }

      RegisterProfileViewModel model = new RegisterProfileViewModel();
      model.accountName = oracleCustomer.accountName;
      model.mobileNumber = oracleCustomer.phoneNumber;
      model.email = oracleCustomer.email;
      
      //model.name = oracleCustomer.accountName;
      return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> RegisterProfile( RegisterProfileViewModel registerModel )
    {
      Console.WriteLine( "Hiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii!" );
      try
      {
        string _accountNumber = HttpContext.Session.GetString( "custRegAccountNumber" );

        //string _accountId = HttpContext.Session.GetString( "custRegAccountNumber" );

        //using( var response = await _httpClient.GetAsync( $"Authentication/Register/{registerModel.name}/{registerModel.accountName}" +
        //  $"/{_accountNumber}/{registerModel.mobileNumber}/{registerModel.email}/{registerModel.designation}/{registerModel.password}" ) )
        //{
        //  string apiResponse = await response.Content.ReadAsStringAsync();
        //  result = JsonConvert.DeserializeObject<bool>( apiResponse );
        //}
        var registerDTO = new
        {
          name = registerModel.name,
          accountName = registerModel.accountName,
          accountNumber = _accountNumber,
          mobileNumber = registerModel.mobileNumber,
          designation = registerModel.designation,
          password = registerModel.password,
          email = registerModel.email
        };
        

        var jsonProfile = JsonConvert.SerializeObject( registerDTO );
        var contentProfile = new StringContent( jsonProfile, Encoding.UTF8, "application/json" );
        bool result;

        using( var response = await _httpClient.PostAsync( $"Authentication/Register/", contentProfile ) )
        {
          string apiResponse = await response.Content.ReadAsStringAsync();
          result = JsonConvert.DeserializeObject<bool>( apiResponse );
        }


        if( result )
        {
          //............get invoices and posting them in sql db............
          List<InvoiceDTO> invoices = new List<InvoiceDTO>();
          using( var response = await _httpClientOracle.GetAsync( $"Invoice/GetCustomerInvoices/{_accountNumber}" ) )
          {
            string apiResponse = await response.Content.ReadAsStringAsync();
            if( JsonConvert.DeserializeObject<List<InvoiceDTO>>( apiResponse ) != null)
            {
              invoices = JsonConvert.DeserializeObject<List<InvoiceDTO>>( apiResponse );
            }
          }

          List<Invoice> custInvoices = new List<Invoice>();
          foreach( var item in invoices )
          {
            Invoice invoice = new Invoice();
            invoice.Id = default;
            invoice.AccountNumber = item.accountNumber;
            invoice.OldAccountId = item.oldAccountId;
            invoice.GlDate = item.glDate;
            invoice.Credit = Convert.ToDecimal( item.credit );
            invoice.Debit = Convert.ToDecimal( item.debit );
            invoice.TrxDate = item.trxDate;
            invoice.RefNo = item.refNo;
            invoice.TransactionClass = item.transactionClass;
            invoice.DocNumber = item.docNumber;
            invoice.CustomerPartyId = item.customerPartyId;
            invoice.CustTrxTypeId = item.custTrxTypeId;
            invoice.AccountName = item.accountName;
            custInvoices.Add( invoice );
          }

          var json = JsonConvert.SerializeObject( custInvoices );
          var content = new StringContent( json, Encoding.UTF8, "application/json" );

          bool invoicesResult;
          using( var response = await _httpClient.PostAsync( $"Invoice/SaveCustomerInvoices/", content ) )
          {
            string apiResponse = await response.Content.ReadAsStringAsync();
            invoicesResult = JsonConvert.DeserializeObject<bool>( apiResponse );
          }

          //...............getting statement and posting it in sql db.................................
          //..........................................................................................
          //..........................................................................................

          List<InvoiceDTO> statementsOracle = new List<InvoiceDTO>();
          using( var response = await _httpClientOracle.GetAsync( $"Invoice/GetStatementsByAccountNumberOracleDB/{_accountNumber}" ) )
          {
            string apiResponse = await response.Content.ReadAsStringAsync();
            if( apiResponse != null )
            {
              if( JsonConvert.DeserializeObject<List<InvoiceDTO>>( apiResponse )  != null)
              {
                statementsOracle = JsonConvert.DeserializeObject<List<InvoiceDTO>>( apiResponse );
              }
            }
          }

          List<Statement> statements = new List<Statement>();
          foreach( var item in statementsOracle )
          {
            Statement invoice = new Statement();
            invoice.Id = default;
            invoice.AccountNumber = item.accountNumber;
            invoice.OldAccountId = item.oldAccountId;
            invoice.GlDate = item.glDate;
            invoice.Credit = Convert.ToDecimal( item.credit );
            invoice.Debit = Convert.ToDecimal( item.debit );
            invoice.TrxDate = item.trxDate;
            invoice.RefNo = item.refNo;
            invoice.TransactionClass = item.transactionClass;
            invoice.DocNumber = item.docNumber;
            invoice.CustomerPartyId = item.customerPartyId;
            invoice.CustTrxTypeId = item.custTrxTypeId;
            invoice.AccountName = item.accountName;
            statements.Add( invoice );
          }

          var jsonStatements = JsonConvert.SerializeObject( statements );
          var contentStatements = new StringContent( jsonStatements, Encoding.UTF8, "application/json" );

          bool statementsResult;
          using( var response = await _httpClient.PostAsync( $"Transaction/SaveCustomerStatements/", contentStatements ) )
          {
            string apiResponse = await response.Content.ReadAsStringAsync();
            statementsResult = JsonConvert.DeserializeObject<bool>( apiResponse );
          }

          //**********************************************************************************************************
          //***************************************** Payments *******************************************
          //**********************************************************************************************************
          List<Payment> payments = new List<Payment>();
          foreach( var stmt in statementsOracle )
          {
            if(stmt.transactionClass == "Payment")
            {
              Payment payment = new Payment();
              payment.Id = default( int );
              payment.Amount = Convert.ToDecimal( stmt.credit);
              payment.CustomerId = stmt.customerPartyId;
              payment.PaymentDate = stmt.trxDate;
              payment.AccountName = stmt.accountName;
              payment.AccountNumber = stmt.accountNumber;
              payment.Status = "Paid";
              payments.Add( payment );
            }
          }

          var jsonPayments = JsonConvert.SerializeObject( payments );
          var contentPayments = new StringContent( jsonPayments, Encoding.UTF8, "application/json" );
          bool paymentResult;
          using( var response = await _httpClient.PostAsync( $"Payment/SaveCustomerPayments/", contentPayments ) )
          {
            string apiResponse = await response.Content.ReadAsStringAsync();
            paymentResult = JsonConvert.DeserializeObject<bool>( apiResponse );
          }

          return RedirectToAction( "Login" );
        }

        return View();
      }
      catch( Exception ex )
      {
        throw new Exception(ex.ToString() );
      }
    }

    public async Task<ActionResult> ForgotPassword()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
    {
      bool result;
      using( var response = await _httpClient.GetAsync( $"Authentication/ForgetPasswordCustomer/{forgotPasswordViewModel.email}" ) )
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

    public async Task<ActionResult<Customer>> GetCurrentCustomer()
    {
      int customerId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault().Value );
      Customer customer = new Customer();
      using( var response = await _httpClient.GetAsync( $"Customer/GetCustomerById/{customerId}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        if( apiResponse != null )
        {
          if( JsonConvert.DeserializeObject<Customer>( apiResponse ) != null)
          {
            customer = JsonConvert.DeserializeObject<Customer>( apiResponse );
          }
        }
      }

      return customer;
    }


  }
}
