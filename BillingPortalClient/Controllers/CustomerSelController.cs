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

        [HttpPost]
        public async Task<IActionResult> GetCustomersByLocation(string location)
        {
            var customers = await _oracleApiServices.GetCustomersByLocation(location);
            var modelViews = new CustomerSelModelViews { Customers = customers.Select(c => (CustomerSelModelViews.CustomerList)c).ToList() };
            return Json(modelViews.Customers);
        }

        [HttpPost]
        public async Task<IActionResult> GetEmailsByCustomer(int customerId)
        {
            var emails = await _oracleApiServices.GetEmailsByCustomer(customerId);
            var modelViews = new CustomerSelModelViews { Emails = MapToViewModelEmails(emails) };
            return Json(modelViews.Emails);
        }

        [HttpPost]
        public async Task<IActionResult> GetAccountsByEmail(int emailId)
        {
            var accounts = await _oracleApiServices.GetAccountsByEmail(emailId.ToString());
            var modelViews = new CustomerSelModelViews { Accounts = accounts.Select(a => (CustomerSelModelViews.AccountList)a).ToList() };
            return Json(modelViews.Accounts);
        }

        private List<CustomerSelModelViews.EmailList> MapToViewModelEmails(List<EmailList> emails)
        {
            return emails.Select(e => new CustomerSelModelViews.EmailList
            {
                Id = e.Id,
                Address = e.Address,
            }).ToList();
        }
    }
}
