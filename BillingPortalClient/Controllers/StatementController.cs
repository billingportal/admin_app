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
      //ClaimsPrincipal claimsUser = HttpContext.User;
      //int _accountId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault( x => x.Type == "accountId" ).Value );
      //string _accountNumber = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "customerAccountNumber" ).Value;
      //string _accountName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "accountName" ).Value;

      //string otpVerifyResult;
      //using( var response = await _httpClient.GetAsync( $"Authentication/VerifyOTP/{otp.email}/{otpCode}" ) )
      //{
      //  otpVerifyResult = await response.Content.ReadAsStringAsync();
      //  //otpVerifyResult = JsonConvert.DeserializeObject<string>( apiResponse );
      //}
      int adminId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminId" ).Value );
      string adminEmail = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminEmail" ).Value;
      string adminRole = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminRole" ).Value;
      string adminFirstName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminFirstName" ).Value;
      string adminLastName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminLastName" ).Value;
      string adminStatus = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminStatus" ).Value;

      //List<Statement> statements = new List<Statement>();
      //using( var response = await _httpClient.GetAsync( $"Transaction/GetCustomerStatementByAccountNumber/{_accountNumber}" ) )
      //{
      //  string apiResponse = await response.Content.ReadAsStringAsync();
      //  statements = JsonConvert.DeserializeObject<List<Statement>>( apiResponse );
      //}
      List<BillingSystem.Service.Statement> allStatements = new List<BillingSystem.Service.Statement>();
      {
        var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
        allStatements = ( await client.GetAllStatementsAsync() ).ToList();
      }
      List<BillingSystem.Service.Customer> customers = new List<BillingSystem.Service.Customer>();
      {
        var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
        customers = ( await client.GetCustomersByAdminIdAsync( adminId ) ).ToList();
      }
      List<string> accountNumbers = new List<string>();
      foreach(var item in customers)
      {
        foreach(var acc in item.Accounts)
        {
          if(!accountNumbers.Contains(acc.AccountNumber))
          {
            accountNumbers.Add(acc.AccountNumber);
          }
        }
      }
      List<int> customerIds = customers.Select( x => x.Id ).ToList();
      List<BillingSystem.Service.Statement> statements = new List<BillingSystem.Service.Statement>();

      foreach(var item in allStatements)
      {
        if(accountNumbers.Contains(item.AccountNumber))
        {
          statements.Add( item );
        }
      }

      StatementViewModel statementViewModel = new StatementViewModel();
      statementViewModel.statements = statements;

      List<StatementRow> statementTable = new List<StatementRow>();
      decimal balanceAmount = 0;
      foreach( var item in statements.OrderBy(x => x.TrxDate).ToList())
      {


        decimal paidAmount = 0;
        string invoiceStatus;

        //if( item.InvoiceStatuses.Count > 0 )
        //{
        //  invoiceStatus = item.InvoiceStatuses.FirstOrDefault().Status;
        //}
        //else
        //{
        //  invoiceStatus = "Unpaid";
        //}

        //if( item.InvoicesPayments.Count > 0 )
        //{
        //  foreach( var invoicePayment in item.InvoicesPayments )
        //  {
        //    paidAmount = Convert.ToDecimal( paidAmount + invoicePayment.AmountPaid );
        //  }
        //}

        //balanceAmount = Convert.ToDecimal( item.Debit - paidAmount );
        balanceAmount = balanceAmount + Convert.ToDecimal( item.Debit) - Convert.ToDecimal( item.Credit);


        StatementRow statementRow = new StatementRow();
        //statementRow.status = invoiceStatus;
        statementRow.id = item.Id;
        statementRow.refNo = item.RefNo;
        statementRow.type = item.TransactionClass;
        statementRow.debit = Convert.ToDecimal( item.Debit);
        statementRow.credit = Convert.ToDecimal( item.Credit);
        //statementRow.paid = paidAmount;
        statementRow.docNumber = item.DocNumber;
        //statementRow.balance = balanceAmount;
        statementRow.createdDate = Convert.ToDateTime( item.TrxDate );
        //statementRow.dueDate = item.TrxDate.Value.AddMonths( 1 );
        //statementRow.total = Convert.ToDecimal( item.Debit );
        statementRow.balance = balanceAmount;
        statementRow.accountNumber = item.AccountNumber;
        statementRow.region = customers.Where( x => x.Accounts.Any( y => y.AccountNumber == item.AccountNumber ) ).FirstOrDefault().Region;

        statementTable.Add( statementRow );


      }

      statementViewModel.statementRows = statementTable.OrderByDescending(x => x.createdDate).ToList();

      int allTransactionsCount = 0;
      decimal debitAmountTotal = 0;
      decimal creditAmountTotal = 0;

      if(statementTable != null && statementTable.Count > 0)
      {
        foreach(var item in statementTable)
        {
          allTransactionsCount = allTransactionsCount + 1;
          debitAmountTotal = debitAmountTotal + item.debit;
          creditAmountTotal = creditAmountTotal + item.credit;
        }
      }

      statementViewModel.allTransactionCount = allTransactionsCount;
      statementViewModel.debitAmountTotal = debitAmountTotal;
      statementViewModel.creditAmountTotal = creditAmountTotal;

      //statementViewModel.accountNumber = _accountNumber;
      //statementViewModel.accountName = _accountName;

      return View(statementViewModel);
    }

    public async Task<ActionResult> RefreshCustomerStatements()
    {
      int _accountId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault( x => x.Type == "accountId" ).Value );
      string _accountNumber = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "customerAccountNumber" ).Value;
      string _accountName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "accountName" ).Value;

      List<InvoiceDTO> statementsOracle = new List<InvoiceDTO>();
      using( var response = await _httpClientOracle.GetAsync( $"Invoice/GetStatementsByAccountNumberOracleDB/{_accountNumber}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        if( apiResponse != null )
        {
          if( JsonConvert.DeserializeObject<List<InvoiceDTO>>( apiResponse ) != null )
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

      RefreshStatementDTO refreshStatementDTO = new RefreshStatementDTO();
      refreshStatementDTO.accountNumber = _accountNumber;
      refreshStatementDTO.statements = statements;

      var jsonStatements = JsonConvert.SerializeObject( refreshStatementDTO );
      var contentStatements = new StringContent( jsonStatements, Encoding.UTF8, "application/json" );

      bool statementsResult;
      using( var response = await _httpClient.PostAsync( $"Transaction/RefreshCustomerStatements/", contentStatements ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        statementsResult = JsonConvert.DeserializeObject<bool>( apiResponse );
      }
      return RedirectToAction( "Index" );
    }
  }
}
