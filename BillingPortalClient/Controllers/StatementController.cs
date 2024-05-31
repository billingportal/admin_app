using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BillingPortalClient.Models;
using Newtonsoft.Json;
using System.Text;

namespace BillingPortalClient.Controllers
{
  public class StatementController : BaseController
  {
    
    public async Task<ActionResult> Index()
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

            // Fetch Invoices
            List<CustomerInvoice> invoices = new List<CustomerInvoice>();
            
              string invoiceRequestUri = new Uri(baseAddress, "Invoice/GetCustomerInvoicesByAccountNumber/" + _accountNumber).ToString();
          using (var response = await _httpClient.GetAsync(invoiceRequestUri))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                invoices = JsonConvert.DeserializeObject<List<CustomerInvoice>>(apiResponse);
            }

            // Fetch Payments
            List<BillingPortalClient.Models.Payment> payments = new List<BillingPortalClient.Models.Payment>(); // Use fully qualified name
               string paymentRequestUri = new Uri(baseAddress, "Payment/GetPaymentsByAccountNumber/" + _accountNumber).ToString();

            using (var response2 = await _httpClient.GetAsync(paymentRequestUri))
            {
                string apiResponse2 = await response2.Content.ReadAsStringAsync();
                payments = JsonConvert.DeserializeObject<List<BillingPortalClient.Models.Payment>>(apiResponse2); // Use fully qualified name
            }

            // Combine Invoices and Payments into StatementDTO
            List<Statement> statements = new List<Statement>();

            statements.AddRange(invoices.Select(invoice => new Statement
            {
                transactionClass = "Invoice",
                docNumber = invoice.DocumentNumber,
                glDate = invoice.TransactionDate,
                trxDate = invoice.TransactionDate,
                debit = (double)invoice.TotalPaidAmount,
                credit = 0,
                customerPartyId =  invoice.CustomerId ?? 0,// Assuming correct property name
                custTrxTypeId = 0, // Assuming correct property name
                refNo = invoice.TransactionNumber.ToString(),
                accountNumber = _accountNumber, // Assuming correct property name
                accountName = _accountName, // Assuming correct property name
                oldAccountId = _newAccountNumber // Assuming correct property name
            }));

            statements.AddRange(payments.Select(payment => new Statement
            {
                transactionClass = "Payment",
                docNumber = payment.InvoiceId.ToString(),
                glDate = payment.PaymentDate,
                trxDate = payment.PaymentDate,
                debit = 0,
                credit = (double)payment.Amount,
                customerPartyId = payment.CustomerId ?? 0,
                custTrxTypeId = 0,
                refNo = payment.DocNumber,
                accountNumber = _accountNumber,
                accountName = _accountName,
                oldAccountId = _newAccountNumber
            }));

            // Generate the StatementViewModel
            StatementViewModel statementViewModel = new StatementViewModel();
            List<StatementRow> statementRows = new List<StatementRow>();

            decimal balanceAmount = 0;
            int counter = 1;

            foreach (var item in statements.OrderBy(x => x.trxDate))
            {
                balanceAmount += Convert.ToDecimal(item.debit) - Convert.ToDecimal(item.credit);
                statementRows.Add(new StatementRow
                {
                    id = counter++,
                    refNo = item.refNo,
                    type = item.transactionClass,
                    debit = Convert.ToDecimal(item.debit),
                    credit = Convert.ToDecimal(item.credit),
                    docNumber = item.docNumber,
                    createdDate = item.trxDate ?? DateTime.MinValue,
                    balance = balanceAmount
                });
            }

            statementViewModel.statementRows = statementRows.OrderByDescending(x => x.createdDate).ToList();
            statementViewModel.allTransactionCount = statementRows.Count;
            statementViewModel.debitAmountTotal = statementRows.Sum(x => x.debit);
            statementViewModel.creditAmountTotal = statementRows.Sum(x => x.credit);
            statementViewModel.accountNumber = _accountNumber;
            statementViewModel.accountName = _accountName;

            return View(statementViewModel);
    }

    public async Task<ActionResult> RefreshCustomerStatements()
    {
     
      return RedirectToAction( "Statement", "Index" );
    }
  }
}
