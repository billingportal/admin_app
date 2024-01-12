using Microsoft.AspNetCore.Mvc;
using BillingSystem.Service;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Data;
using System.Threading.Tasks;

namespace BillingPortalClient.Controllers
{
  public class CustomerController : BaseController
  {
    public async Task<ActionResult> Index()
    {
      int adminId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminId" ).Value );
      string adminRole = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminRole" ).Value;

      List<BillingSystem.Service.Customer> customers = new List<BillingSystem.Service.Customer>();
      if( adminRole == "SuperAdmin" )
      {
        var client = new Client( baseUrl, _httpClient );
        customers = ( await client.GetAllCustomerAsync() ).ToList();
      }
      else
      {
        {
          var client = new Client( baseUrl, _httpClient );
          customers = ( await client.GetCustomersByAdminIdAsync( adminId ) ).ToList();
        }
      }
      CustomerViewModel customerViewModel = new CustomerViewModel();
      customerViewModel.Customers = customers;

      List<CustomerRow> customersRows = new List<CustomerRow>();


      foreach( var customer in customers )
      {
        if( customer.IsMain == true )
        {
          foreach( var account in customer.Accounts )
          {
            CustomerRow customerRow = new CustomerRow();
            customerRow.Id = customer.Id;
            customerRow.AccountNumber = account.AccountNumber;
            customerRow.Name = customer.Name;
            customerRow.Email = customer.Email;
            customerRow.IsMain = Convert.ToBoolean( customer.IsMain );
            customerRow.Status = customer.Status;
            customerRow.Region = customer.Region;
            customersRows.Add( customerRow );

            if( account.CustomerUsers.Count > 0 )
            {
              foreach( var custUser in account.CustomerUsers )
              {
                customersRows.Add( new CustomerRow()
                {
                  Id = custUser.Customer.Id,
                  Name = custUser.Customer.Name,
                  AccountNumber = account.AccountNumber,
                  Email = custUser.Customer.Email,
                  IsMain = Convert.ToBoolean( custUser.Customer.IsMain ),
                  Status = custUser.Customer.Status,
                  Region = custUser.Customer.Region,
                } );
              }
            }

          }
        }
      }

      customerViewModel.CustomerRows = customersRows;

      return View( customerViewModel );
    }


    public async Task<ActionResult> SetCustomerStatus( int customerId, string status, string customeremail )
    {
      bool result = false;
      {
        var client = new Client( baseUrl, _httpClient );
        result = await client.SetCustomerStatusAsync(customerId, status);
        SendEmail(customerId, status, customeremail );
      }

      return RedirectToAction( "Index", "Customer" );
    }
 public async void SendEmail(  int customerId, string status, string customeremail )
    {
      var apiKey = "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";
      var client = new SendGridClient( apiKey );
      var from = new EmailAddress( "billingsystem1@yopmail.com", "Billing System" );
      var subject = "User Account Suspended";
      var to = new EmailAddress( customeremail, "billingSystem@gmail.com" );
      var plainTextContent = "Admin has suspended the account of "+ customerId;
      var htmlContent = "<p>Kindly contact admin for further details.</p>";
      var msg = MailHelper.CreateSingleEmail( from, to, subject, plainTextContent, htmlContent );
      var response = await client.SendEmailAsync( msg );
    }
    
  }
}
