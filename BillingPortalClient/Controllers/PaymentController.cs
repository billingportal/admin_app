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

    
    public async Task<ActionResult> Payments()
    {
        var claims = HttpContext.User.Claims;

            string customerIdClaim = claims.FirstOrDefault(x => x.Type == "custCustomerId")?.Value;
            string customerId = string.IsNullOrEmpty(customerIdClaim) ? string.Empty : customerIdClaim;

             
            string accountNumberClaim = claims.FirstOrDefault(x => x.Type == "custAccountNumber")?.Value;
            string _accountNumber = string.IsNullOrEmpty(accountNumberClaim) ? string.Empty : accountNumberClaim;

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
            Console.WriteLine($"_accountNumber: {_accountNumber}");
            Console.WriteLine($"_newAccountNumber: {_newAccountNumber}");
            Console.WriteLine($"_accountId: {_accountId}");
            Console.WriteLine($"_accountName: {_accountName}");
            Console.WriteLine($"_arabicName: {_arabicName}");
            Console.WriteLine($"_businessUnitId: {_businessUnitId}");

      List<Models.Payment> payments = new List<Models.Payment>();

      using( var response = await _httpClient.GetAsync( $"Payment/GetPaymentsByAccountNumber/{_accountNumber}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"payments {apiResponse}");
        payments = JsonConvert.DeserializeObject<List<Models.Payment>>( apiResponse );
        Console.WriteLine($"Deserialized Payments Count: {payments.Count}");
      }
      Console.WriteLine($"payments {payments}");
      PaymentViewModel paymentViewModel = new PaymentViewModel();
      List<PaymentRow> paymentRows = new List<PaymentRow>();
     
      int counter1 = 1;
      foreach(var item2 in payments)
      {
       Console.WriteLine($"Processing Payment: {item2}");

        PaymentRow paymentRow1 = new PaymentRow();
        paymentRow1.id = counter1++;
        paymentRow1.paymentRef = item2.DocNumber;
        paymentRow1.accountName = _accountName;
        paymentRow1.paymentDate = Convert.ToDateTime( item2.PaymentDate );
        paymentRow1.receiverBank = item2.InvoiceId?.ToString() ?? "N/A";
        paymentRow1.paymentAmount = Convert.ToDecimal( item2.Amount);
        paymentRow1.paymentMode = item2.PaymentMethod;
        paymentRows.Add(paymentRow1);
        
      }
      paymentViewModel.paymentRows = paymentRows.OrderByDescending(x => x.paymentDate).ToList();

      int allReceipts = 0;
      decimal sumReceipts = 0;
      decimal paidAmountReceiptsTotal = 0;
      decimal receivedAmountReceiptsTotal = 0;

      if(paymentRows != null && paymentRows.Count > 0)
      {
        foreach(var item in paymentRows)
        {
          allReceipts = allReceipts + 1;
          sumReceipts = sumReceipts + item.paymentAmount;

        }
      }

      paymentViewModel.allReceiptsCount = allReceipts;
      paymentViewModel.sumReceiptsCount = sumReceipts;
      paymentViewModel.accountName = _accountName;
      paymentViewModel.accountNumber = _accountNumber;
      paymentViewModel.businessUnitId = _businessUnitId;


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

   
  }
}
