using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using BillingPortalClient.Models;
using BillingPortalClient.ModelViews;
using BillingSystem.Service;


namespace BillingPortalClient.Controllers
{
  public class InvoiceController : BaseController
  {
    private readonly HttpClient _httpClient;

       public InvoiceController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ActionResult> Index()
    {

            var claims = HttpContext.User.Claims;

            string customerIdClaim = claims.FirstOrDefault(x => x.Type == "custCustomerId")?.Value;
            string customerId = string.IsNullOrEmpty(customerIdClaim) ? string.Empty : customerIdClaim;

             
            string accountNumberClaim = claims.FirstOrDefault(x => x.Type == "custAccountNumber")?.Value;
            string customerAccountNumber = string.IsNullOrEmpty(accountNumberClaim) ? string.Empty : accountNumberClaim;

            string newAccountNumberClaim = claims.FirstOrDefault(x => x.Type == "custNewAccountNumber")?.Value;
            string _newAccountNumber = string.IsNullOrEmpty(newAccountNumberClaim) ? string.Empty : newAccountNumberClaim;

            string accountIdClaim = claims.FirstOrDefault(x => x.Type == "custAccountId")?.Value;
            int _accountId = string.IsNullOrEmpty(accountIdClaim) ? 0 : Convert.ToInt32(accountIdClaim);

            string accountNameClaim = claims.FirstOrDefault(x => x.Type == "custAccountName")?.Value;
            string _accountName = string.IsNullOrEmpty(accountNameClaim) ? string.Empty : accountNameClaim;

            string arabicNameClaim = claims.FirstOrDefault(x => x.Type == "custArabicName")?.Value;
            string _arabicName = string.IsNullOrEmpty(arabicNameClaim) ? string.Empty : arabicNameClaim;

            string businessUnitIdClaim = claims.FirstOrDefault(x => x.Type == "custBusinessUnitId")?.Value;
            string _businessUnitId = string.IsNullOrEmpty(businessUnitIdClaim) ? string.Empty : businessUnitIdClaim;

            Console.WriteLine($"customerId: {customerId}");
            Console.WriteLine($"customerAccountNumber: {customerAccountNumber}");
            Console.WriteLine($"_newAccountNumber: {_newAccountNumber}");
            Console.WriteLine($"_accountId: {_accountId}");
            Console.WriteLine($"_accountName: {_accountName}");
            Console.WriteLine($"_arabicName: {_arabicName}");
            Console.WriteLine($"_businessUnitId: {_businessUnitId}");
            // Fetch customer invoices
            List<CustomerInvoice> invoices;
            string requestUri = new Uri(baseAddress, "Invoice/GetCustomerInvoicesByAccountNumber/" + customerAccountNumber).ToString();
            using (var response = await _httpClient.GetAsync(requestUri))
              {
                string apiResponse = await response.Content.ReadAsStringAsync();
                 Console.WriteLine($"invoices: {apiResponse}");
                invoices = JsonConvert.DeserializeObject<List<CustomerInvoice>>(apiResponse);    
                   
            }

            if (invoices != null)
            {

              Console.WriteLine($"invoices not null: {invoices}");
                // Map DTO to ViewModel
                var invoiceViewModel = new CustomerInvoiceViewModel
                {
                    invoices = invoices,
                    accountNumber = customerAccountNumber,
                    accountName = _accountName,
                    arabicName = _arabicName,
                    newAccountNumber = _newAccountNumber,
                    businessUnitId = _businessUnitId,
                    invoiceTable = invoices.Select(item => new InvoiceRow
                    {
                        status = item.InvoicePaymentStatus, // Updated property name
                        id = item.TransactionNumber,
                        paid = (double)item.TotalPaidAmount, // Updated property name
                        docNumber = item.DocumentNumber,
                        balance = (double)item.InvoiceBalanceAmount,
                        invoiceDate = item.TransactionDate,
                        dueDate = item.DueDate,
                        total = (double)item.EnteredAmount
                    }).OrderByDescending(x => x.invoiceDate).ToList()
                };

                // Serialize the object to a JSON string
                string invoiceViewModelJson = JsonConvert.SerializeObject(invoiceViewModel, Formatting.Indented);

                // Log the JSON string to the console
                Console.WriteLine(invoiceViewModelJson);

                // Fetch disputed invoices
                List<CustomerInvoice> disputedInvoices;
                string requestUri2 = new Uri(baseAddress, "Invoice/GetCustomerDisputedInvoicesByAccountId/" + _accountId).ToString();
                using (var response2 = await _httpClient.GetAsync(requestUri2))
                {
                     string apiResponse2 = await response2.Content.ReadAsStringAsync();
                    disputedInvoices = JsonConvert.DeserializeObject<List<CustomerInvoice>>(apiResponse2);
                }

                if (disputedInvoices != null){
                    invoiceViewModel.disputedInvoices = disputedInvoices;
                }else {
                  invoiceViewModel.disputedInvoices = new List<CustomerInvoice>();
                }

                

                // Calculate open transactions and overdue amount
                double? openTransactions = invoiceViewModel.invoiceTable.Sum(item => item.total);
                double? overdueAmount = invoiceViewModel.invoiceTable.Where(item => item.dueDate < DateTime.Now.Date).Sum(item => item.balance);

                // Handle null values and convert them to default (0) if needed
                invoiceViewModel.openTransaction = openTransactions.HasValue ? Math.Round(openTransactions.Value, 2) : 0;
                invoiceViewModel.overdueAmount = overdueAmount.HasValue ? Math.Round(overdueAmount.Value, 2) : 0;
                
                Console.WriteLine($"overdueAmount: {invoiceViewModel.overdueAmount}");
                return View("Index", invoiceViewModel);
            }
            else
            {
                // Handle the case when invoices are null
                // You can redirect to an error page or return a specific view for this case
                
                List<InvoiceRow> InvoiceRows = new List<InvoiceRow>();
                
                 var invoiceViewModel = new CustomerInvoiceViewModel
                {
                    invoices = invoices,
                    accountNumber = customerAccountNumber,
                    accountName = _accountName,
                    arabicName = _arabicName,
                    newAccountNumber = _newAccountNumber,
                    businessUnitId = _businessUnitId,
                    invoiceTable = InvoiceRows
                };
                List<CustomerInvoice> disputedInvoices= new List<CustomerInvoice>();
                invoiceViewModel.disputedInvoices = disputedInvoices;

                // Calculate open transactions and overdue amount
                double? openTransactions = invoiceViewModel.invoiceTable.Sum(item => item.total);
                double? overdueAmount = invoiceViewModel.invoiceTable.Where(item => item.dueDate < DateTime.Now.Date).Sum(item => item.balance);

                // Handle null values and convert them to default (0) if needed
                invoiceViewModel.openTransaction = openTransactions.HasValue ? Math.Round(openTransactions.Value, 2) : 0;
                invoiceViewModel.overdueAmount = overdueAmount.HasValue ? Math.Round(overdueAmount.Value, 2) : 0;
               return View("Index", invoiceViewModel);
            }

    }

 

     [HttpPost]
    public async Task<IActionResult> DownloadInvoiceReport(string buId, string startDate, string endDate, string attachmentType, string customerAccountNumber)
    {
        try
        {
            var request = new
            {
                buId = buId,
                startDate = startDate,
                endDate = endDate,
                attachmentType = attachmentType,
                customerAccountNumber = customerAccountNumber
            };
            
            // Construct the request URI
            string requestUri = new Uri(baseAddress, "Invoice/processAccountSuppressReport").ToString();

            // Send the POST request with the JSON payload
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(requestUri, request);
            response.EnsureSuccessStatusCode();

            // Read the response content as JSON string
            string apiResponse = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON response
            ApiResponse responseData = JsonConvert.DeserializeObject<ApiResponse>(apiResponse);

            return Json(responseData);
        }
        catch (Exception ex)
        {
            return Json(new ApiResponse { result = "Error", message = ex.Message });
        }
    }


    [HttpPost]
    public async Task<IActionResult> DownloadReceiptReport(string buId, string attachmentType, string customerAccountNumber, string transactionType, string transactionNumber)
    {
        try
        {
            var request = new
            {
                buId = buId,
                attachmentType = attachmentType,
                customerAccountNumber = customerAccountNumber,
                transactionType = transactionType,
                transactionNumber = transactionNumber
            };
            
            // Construct the request URI
            string requestUri = new Uri(baseAddress, "Invoice/processArInvoiceReport").ToString();

            // Send the POST request with the JSON payload
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(requestUri, request);
            response.EnsureSuccessStatusCode();

            // Read the response content as JSON string
            string apiResponse = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON response
            ApiResponse responseData = JsonConvert.DeserializeObject<ApiResponse>(apiResponse);

            return Json(responseData);
        }
        catch (Exception ex)
        {
            return Json(new ApiResponse { result = "Error", message = ex.Message });
        }
    }
    

    private string GetInvoiceNumbers(PaymentViewModel paymentViewModel)
{
    // Assuming selectedInvoices is a list of invoice numbers
    string invoiceNumbers = string.Join(", ", paymentViewModel.selectedInvoices);
    return invoiceNumbers;
}

        public async Task<IActionResult> RefreshInvoices()
        {
        return RedirectToAction(nameof(Index));
        }




    
  }
}
