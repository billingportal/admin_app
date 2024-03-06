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
        public async Task<IActionResult> GetSuggestions(string keyword)
        {
            var customers = await _oracleApiServices.GetSuggestionsByKeyword(keyword);
            var modelViews = new CustomerSelModelViews { Customers = customers.Select(cl => (CustomerSelModelViews.CustomerList)cl).ToList() };
          
            return Json(modelViews.Customers);
        }

       
        [HttpPost]
        public async Task<IActionResult> GetCustomersByCompany(string company)
        {
            var customers = await _oracleApiServices.GetCustomersByCompany(company);
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
