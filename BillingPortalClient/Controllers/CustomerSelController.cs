// BillingPortalClient\Controllers\CustomerSelController.cs

using Microsoft.AspNetCore.Mvc;
using BillingPortalClient.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingPortalClient.Services;
using static BillingPortalClient.ModelViews.CustomerSelModelViews;

namespace BillingPortalClient.Controllers
{
    public class CustomerSelController : Controller
    {
        private readonly OracleApiServices _oracleApiServices;

        public CustomerSelController(OracleApiServices oracleApiServices)
        {
            _oracleApiServices = oracleApiServices;
        }

        public IActionResult Index()
        {
            return View(new CustomerSelModelViews());
        }

        [HttpGet]
        public async Task<JsonResult> GetAllCustomersRegion()
        {
            try
            {
                int adminId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "adminId").Value);
                string adminRole = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "adminRole").Value;

                Console.WriteLine($"Get All Customers Oracle");

                List<string> allRegions = new List<string>();

                if (adminRole == "SuperAdmin")
                {
                    Console.WriteLine($"User is SuperAdmin");
                    var customersList = await _oracleApiServices.GetAllCustomers();
                    Console.WriteLine($"customersList Count: {customersList.Count}");

                    // Debug: Print details of the first customer in the list
                    var firstCustomer = customersList.FirstOrDefault();
                    if (firstCustomer != null)
                    {
                        Console.WriteLine($"Customer: {firstCustomer.accountName}, {firstCustomer.accountNumber}, {firstCustomer.email}, ...");
                    }

                    // Extract regions from customers and accounts
                    allRegions = customersList
                    .Select(c => c.region)
                    .Distinct()
                    .ToList();

                    Console.WriteLine($"allRegions Count: {allRegions.Count}");

                    // Debug: Print details of the first few regions
                    Console.WriteLine($"First few regions: {string.Join(", ", allRegions.Take(5))}");
                    Console.WriteLine($"Regions of Customers Oracle Connection: {string.Join(", ", allRegions)}");
                }
                else
                {
                    var customersList = await _oracleApiServices.GetAllCustomersByAdminId(adminId);
                    // Extract regions from customers and accounts
                    allRegions = customersList
                    .Select(c => c.region)
                    .Distinct()
                    .ToList();
                }

                return Json(allRegions);
            }
            catch (Exception ex)
            {
                // Handle exception appropriately (e.g., log it)
                return Json(new { error = "An error occurred while fetching regions." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetCustomersByLocation(string location)
        {
              Console.WriteLine($"Requesting Customers for Location {location}");
            var customers = await _oracleApiServices.GetCustomersByLocation(location);
             Console.WriteLine($"Response {customers}");
            var modelViews = new CustomerSelModelViews { Customers = customers.Select(cl => (CustomerSelModelViews.CustomerList)cl).ToList() };
            return Json(modelViews.Customers);
        }

        [HttpPost]
        public async Task<IActionResult> GetEmailsByCustomer(string accountNumber)
        {
            var emails = await _oracleApiServices.GetEmailsByCustomer(accountNumber);
            var modelViews = new CustomerSelModelViews { Emails = MapToViewModelEmails(emails) };
            return Json(modelViews.Emails);
        }

        private List<CustomerSelModelViews.EmailList> MapToViewModelEmails(List<EmailList> Emails)
        {
            return Emails.Select(e => new CustomerSelModelViews.EmailList
            {
                Id = e.Id,
                email = e.email,
            }).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> GetAccountsByEmail(string email)
        {
            var accounts = await _oracleApiServices.GetAccountsByEmail(email);
            var modelViews = new CustomerSelModelViews { Accounts = accounts.Select(ae => (CustomerSelModelViews.AccountList)ae).ToList() };
            return Json(modelViews.Accounts);
        }

        
    }
}
