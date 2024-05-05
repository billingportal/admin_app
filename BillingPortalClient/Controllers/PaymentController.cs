using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using BillingPortalClient.Models;
using BillingSystem.Service;

namespace BillingPortalClient.Controllers
{
  public class PaymentController : BaseController
  {

    [HttpPost]
    public async Task<ActionResult<bool>> SavePayment([FromBody] PaymentViewModel paymentViewModel)
    {
      // ClaimsPrincipal claimsUser = HttpContext.User;
      string _accountNumber = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "customerAccountNumber").Value;
      paymentViewModel.accountNumber = _accountNumber;

      var json = JsonConvert.SerializeObject(paymentViewModel);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      bool invoicesResult;
      using (var response = await _httpClient.PostAsync($"Payment/SavePayment/", content))
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        invoicesResult = JsonConvert.DeserializeObject<bool>(apiResponse);
      }

      // Check if the request is coming from the return URL
      if (IsReturnUrlRequest())
      {
        // Display success message and transaction details in a modal
        ViewBag.SuccessMessage = "Payment successful!";
        ViewBag.TransactionDetails = "Transaction ID: " + paymentViewModel.paymentTransactionId + "\nAmount: " + paymentViewModel.paymentAmount;

        // Call middleware app to run stored procedure
        await CallMiddlewareAppAsync(paymentViewModel.paymentTransactionId, paymentViewModel.paymentAmount);

        // Redirect to the Payments view with the success message
        return RedirectToAction("Payments");
      }

      return true;
    }

    private bool IsReturnUrlRequest()
    {
      // Check some condition to determine if the request is from the return URL
      // For example, check if the request has a specific query parameter or header
      // Adjust this logic based on your actual implementation
      return HttpContext.Request.Query.ContainsKey("returnFromSavePayment");
    }
    private async Task CallMiddlewareAppAsync(string transactionId, decimal amount)
    {
      
      // Assuming you have a URL endpoint in your middleware app to trigger the stored procedure
      //string middlewareAppUrl = "https://middleware-app-url/trigger-stored-procedure";

       string middlewareAppUrl = "http://88.85.242.29/api/savepaymentoracle";
      // Prepare data to send to the middleware app
      var middlewareData = new { TransactionId = transactionId, Amount = amount };
      var jsonMiddlewareData = JsonConvert.SerializeObject(middlewareData);
      var contentMiddleware = new StringContent(jsonMiddlewareData, Encoding.UTF8, "application/json");

      // Send data to the middleware app
      using (var response = await _httpClient.PostAsync(middlewareAppUrl, contentMiddleware))
      {
        // Handle the response if needed
      }
    }

    public async Task<ActionResult> Payments()
    {
      ClaimsPrincipal claimsUser = HttpContext.User;
      //string _accountNumber = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "customerAccountNumber" ).Value;
      //string _accountName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "accountName" ).Value;
      int adminId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "adminId").Value);
      string adminEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "adminEmail").Value;
      string adminRole = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "adminRole").Value;
      string adminFirstName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "adminFirstName").Value;
      string adminLastName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "adminLastName").Value;
      string adminStatus = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "adminStatus").Value;

      List<BillingSystem.Service.Payment> allPayments = new List<BillingSystem.Service.Payment>();
      {
        var client = new Client(baseUrl, _httpClient);
        allPayments = (await client.GetAllPaymentsAsync()).ToList();
      }
      Console.WriteLine($"All Payments Count: {allPayments.Count}");
      List<BillingSystem.Service.Customer> customers = new List<BillingSystem.Service.Customer>();
      {
        var client = new Client(baseUrl, _httpClient);
        customers = (await client.GetCustomersByAdminIdAsync(adminId)).ToList();
      }
       Console.WriteLine($"All Customers Count: {customers.Count}");
      List<int> customerIds = customers.Select(x => x.Id).ToList();
      List<BillingSystem.Service.Payment> payments = new List<BillingSystem.Service.Payment>();
      foreach (var item in allPayments)
      {
        if (customerIds.Contains((int)item.CustomerId))
        {
          payments.Add(item);
        }
      }

      PaymentViewModel paymentViewModel = new PaymentViewModel();
      List<PaymentRow> paymentRows = new List<PaymentRow>();

      foreach (var item in payments)
      {
        PaymentRow paymentRow = new PaymentRow();
        paymentRow.id = item.Id;
        paymentRow.accountName = item.AccountName;
        paymentRow.paymentDate = Convert.ToDateTime(item.PaymentDate);
        paymentRow.receiverBank = "Null";
        paymentRow.paymentAmount = Convert.ToDecimal(item.Amount);
        paymentRow.paymentMode = item.PaymentMethod;
        paymentRow.region = "";
        paymentRow.accountNumber = item.AccountNumber;
        paymentRows.Add(paymentRow);
      }
      paymentViewModel.paymentRows = paymentRows.OrderByDescending(x => x.paymentDate).ToList();

      int allReceipts = 0;
      decimal paidAmountReceiptsTotal = 0;
      decimal receivedAmountReceiptsTotal = 0;

      if (paymentRows != null && paymentRows.Count > 0)
      {
        foreach (var item in paymentRows)
        {
          allReceipts = allReceipts + 1;
        }
      }

      paymentViewModel.allReceiptsCount = allReceipts;

      //paymentViewModel.accountName = _accountName;
      //paymentViewModel.accountNumber = _accountNumber;


      return View(paymentViewModel);
    }

    public async Task<ActionResult> RefreshCustomerPayments()
    {
      string _accountNumber = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "customerAccountNumber").Value;

      List<InvoiceDTO> statementsOracle = new List<InvoiceDTO>();
      using (var response = await _httpClientOracle.GetAsync($"Invoice/GetStatementsByAccountNumberOracleDB/{_accountNumber}"))
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        if (apiResponse != null)
        {
          if (JsonConvert.DeserializeObject<List<InvoiceDTO>>(apiResponse) != null)
          {
            statementsOracle = JsonConvert.DeserializeObject<List<InvoiceDTO>>(apiResponse);
          }
        }
      }

      List<Models.Payment> payments = new List<Models.Payment>();
      foreach (var stmt in statementsOracle)
      {
        if (stmt.transactionClass == "Payment")
        {
          Models.Payment payment = new Models.Payment();
          payment.Id = default(int);
          payment.Amount = Convert.ToDecimal(stmt.credit);
          payment.CustomerId = stmt.customerPartyId;
          payment.PaymentDate = stmt.trxDate;
          payment.AccountName = stmt.accountName;
          payment.AccountNumber = stmt.accountNumber;
          payment.Status = stmt.status;
          payments.Add(payment);
        }
      }

      Models.RefreshPaymentDTO refreshPaymentDTO = new Models.RefreshPaymentDTO();
      refreshPaymentDTO.accountNumber = _accountNumber;
      refreshPaymentDTO.payments = payments;

      var jsonPayments = JsonConvert.SerializeObject(refreshPaymentDTO);
      var contentPayments = new StringContent(jsonPayments, Encoding.UTF8, "application/json");
      bool paymentResult;
      using (var response = await _httpClient.PostAsync($"Payment/RefreshCustomerPayments/", contentPayments))
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        paymentResult = JsonConvert.DeserializeObject<bool>(apiResponse);
      }

      return RedirectToAction("Payments");
    }

    [HttpPost]
    public async Task<ActionResult<string>> StartPayfortPayment([FromBody] PaymentViewModel paymentViewModel)
    {
      int customerId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault().Value);
      int _accountId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "accountId").Value);
      string _accountName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "accountName").Value;
      bool _isMain = Convert.ToBoolean(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "isMain").Value);
      string _accountNumber = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "customerAccountNumber").Value;



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

      string merchantReference = _accountNumber + "_" + DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss");
      var client = new Client(baseUrl, _httpClient);
      try
      {
        MerchantInvoiceReferenceDTO merchantInvoiceReferenceDTO = new MerchantInvoiceReferenceDTO();
        merchantInvoiceReferenceDTO.MerchantReference = merchantReference;
        merchantInvoiceReferenceDTO.InvoiceNumbers = paymentViewModel.selectedInvoices;

        await client.SaveMerchantInvoiceReferenceAsync(merchantInvoiceReferenceDTO);

      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }

      paymentRequestView.UniqueOrderReference = merchantReference;


      paymentRequestView.MerchantId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");
      paymentRequestView.GatewayOption = "PAYFORT";
      paymentRequestView.GatewayId = 0;
      paymentRequestView.Language = "en";
      paymentRequestView.PaymentType = "string";
      paymentRequestView.UserAgent = "string";
      paymentRequestView.CurrencyCode = "AED";
      paymentRequestView.Amount = Convert.ToDouble(paymentViewModel.paymentAmount);

      paymentRequestView.Channel = "string";
      paymentRequestView.Email = "test1@abc.com";
      paymentRequestView.Mobile = "string";
      paymentRequestView.CustomerName = "string";
      paymentRequestView.Token = "string1";
      paymentRequestView.ReturnURL = "https://billingportalwebapp.azurewebsites.net/Payments/Paymenta";
      //paymentRequestView.ReturnURL = "https://05b5-139-135-59-47.ngrok-free.app/Home/CompeletePaymentAmazon";
      paymentRequestView.Mode = "string";
      paymentRequestView.PaymentOption = "string";
      paymentRequestView.CardNumber = "string";
      paymentRequestView.Description = "string";
      paymentRequestView.UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");
      paymentRequestView.Otp = "string";



      var json = JsonConvert.SerializeObject(paymentRequestView);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      Models.PaymentResponseView paymentResponseView = new Models.PaymentResponseView();
      using (var response = await _httpClient.PostAsync($"PaymentGateWay/IntializePayment/", content))
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        if (apiResponse != null)
        {
          try
          {
            paymentResponseView = JsonConvert.DeserializeObject<Models.PaymentResponseView>(apiResponse);

          }
          catch (Exception ex)
          {
            throw new Exception(ex.Message);
          }
        }
      }

      await client.SaveCurrentUserContextAsync(_accountNumber, _accountId, customerId, _accountName, _isMain, merchantReference);

      return paymentResponseView.PostData;
    }
  }
}
