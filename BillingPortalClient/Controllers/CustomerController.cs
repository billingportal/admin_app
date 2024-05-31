using Microsoft.AspNetCore.Mvc;
using BillingSystem.Service;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Data;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using BillingPortalClient.Models;

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

   public async Task<List<CustomerAccount>> GetSuggestionsByKeyword(string keyword, string[] salesRegions)
    {
        try
        {
            // Call the middleware API endpoint
            var content = new StringContent(JsonConvert.SerializeObject(new { keyword, salesRegions }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Customer/GetSuggestionsByKeyword", content);

            // Check if response is successful
           if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    var responseBody = await response.Content.ReadAsStringAsync();

                   // Deserialize the response content into a list of Account objects
                  var accounts = JsonConvert.DeserializeObject<List<CustomerAccount>>(responseBody, new JsonSerializerSettings
                  {
                      NullValueHandling = NullValueHandling.Ignore // Ignore null values during deserialization
                  });

                  return accounts;
                }
                else
                {
                    // Handle the case where the API returns an error status code
                    Console.WriteLine($"Error fetching customers. Status code: {response.StatusCode}");
                }

                // Handle other exceptions (e.g., HttpRequestException) as needed
                throw new Exception($"Error fetching customers. Status code: {response.StatusCode}");
            }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request exception: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }





    
  }
}
