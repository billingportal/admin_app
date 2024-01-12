using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//using BillingPortalClient.Models;
using BillingPortalClient.ModelViews;
using System.Text;
using BillingSystem.Service;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace BillingPortalClient.Controllers
{
  public class InvoiceController : BaseController
  {
    ////Uri baseAddress = new Uri( "https://localhost:7069/api/" );
    //Uri baseAddress = new Uri( "https://billingportalapis.azurewebsites.net/" );

    //private readonly HttpClient _httpClient;

    //public InvoiceController()
    //{
    //  _httpClient = new HttpClient();
    //  _httpClient.BaseAddress = baseAddress;
    //}

    public async Task<ActionResult> Index()
    {
      try
      {
        int adminId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminId" ).Value );
        string adminEmail = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminEmail" ).Value;
        string adminRole = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminRole" ).Value;
        string adminFirstName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminFirstName" ).Value;
        string adminLastName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminLastName" ).Value;
        string adminStatus = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminStatus" ).Value;

        string otpVerifyResult;

        //List<Invoice> invoices = new List<Invoice>();
        List<InvoiceRowDTO> invoices = new List<InvoiceRowDTO>();
        //using( var response = await _httpClient.GetAsync( $"Invoice/GetCustomerInvoicesByAccountNumber/{customerAccountNumber}" ) )
        //{
        //  string apiResponse = await response.Content.ReadAsStringAsync();
        //  invoices = JsonConvert.DeserializeObject<List<Invoice>>( apiResponse );
        //}

        //if(adminRole == "SuperAdmin")
        //{
        //  var client = new Client( baseUrl, _httpClient );
        //  invoices = ( await client.GetAllInvoicesAsync() ).ToList();
        //}
        //else
        //{
        //  var client = new Client( baseUrl, _httpClient );
        //  invoiceRowDTOs = ( await client.GetInvoicesByAdminIdAsync(adminId) ).ToList();
        //}

        {
          var client = new Client( baseUrl, _httpClient );
          invoices = ( await client.GetInvoicesByAdminIdAsync( adminId ) ).ToList();
        }

        InvoiceViewModel invoiceViewModel = new InvoiceViewModel();
        //invoiceViewModel.invoices = invoices;
        invoiceViewModel.invoiceRowDTOs = invoices;

        List<InvoiceRow> invoicesTable = new List<InvoiceRow>();
        foreach( var item in invoices )
        {


          double? paidAmount = 0;
          double? balanceAmount = 0;
          string invoiceStatus;

          //if( item.InvoiceStatuses.Count > 0 )
          //{
          //  invoiceStatus = item.InvoiceStatuses.FirstOrDefault().Status;
          //}
          //else
          //{
          //  invoiceStatus = "Unpaid";
          //}

          if( item.InvoicesPayments.Count > 0 )
          {
            foreach( var invoicePayment in item.InvoicesPayments )
            {
              paidAmount = Convert.ToDouble( paidAmount + invoicePayment.AmountPaid );
            }
          }

          balanceAmount = item.Debit - paidAmount;

          InvoiceRow invoiceRow = new InvoiceRow();
          invoiceRow.status = item.Status;
          invoiceRow.id = item.Id;
          invoiceRow.paid = paidAmount;
          invoiceRow.docNumber = item.DocNumber;
          invoiceRow.balance = balanceAmount;
          invoiceRow.invoiceDate = Convert.ToDateTime( item.TrxDate );
          invoiceRow.dueDate = Convert.ToDateTime( item.DueDate );
          invoiceRow.total = item.Debit;
          invoiceRow.region = item.Region;
          invoiceRow.accountNumber = item.AccountNumber;

          invoicesTable.Add( invoiceRow );

        }

        invoiceViewModel.invoiceTable = invoicesTable.OrderByDescending( x => x.invoiceDate ).ToList();

        List<BillingSystem.Service.Invoice> disputedInvoices = new List<BillingSystem.Service.Invoice>();
        {
          var client = new Client( baseUrl, _httpClient );
          disputedInvoices = ( await client.GetDisputedInvoicesByAdminIdAsync(adminId) ).ToList();
        }

        if( disputedInvoices != null )
        {
          invoiceViewModel.disputedInvoices = disputedInvoices;
        }

        //invoiceViewModel.accountNumber = customerAccountNumber;
        //invoiceViewModel.accountName = _accountName;

        double openTransactions = 0;
        foreach(var item in invoicesTable)
        {
          openTransactions = openTransactions + (double)item.total;
        }
        invoiceViewModel.openTransactions = Math.Round( openTransactions, 2 );

        double overdueAmount = 0;
        foreach(var item in invoicesTable)
        {
          if(item.dueDate.Date < DateTime.Now.Date)
          {
            overdueAmount = overdueAmount + (double)item.balance;
          }
        }
        invoiceViewModel.overdueAmount = Math.Round( overdueAmount, 2 );

        return View( invoiceViewModel );
      }
      catch( Exception ex )
      {
        throw new Exception(ex.Message );
      }
    }

    public async Task<IActionResult> PaymentType()
    {
      return View();
    }

    public async Task<IActionResult> PaymentMethod()
    {
      return View();
    }

    public async Task<IActionResult> PaymentSummary()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult<bool>> testAjax([FromBody] testAjax a)
    {

      return true;
    }

    //public async Task<ActionResult> RefreshInvoices()
    //{
    //  int _customerId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault().Value );
    //  string _accountNumber = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "customerAccountNumber" ).Value;
    //  int _accountId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault( x => x.Type == "accountId" ).Value );
    //  string _accountName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "accountName" ).Value;
    //  bool _isMain = Convert.ToBoolean( HttpContext.User.Claims.FirstOrDefault( x => x.Type == "isMain" ).Value );

    //  List<InvoiceDTO> invoices = new List<InvoiceDTO>();
    //  using( var response = await _httpClientOracle.GetAsync( $"Invoice/GetCustomerInvoices/{_accountNumber}" ) )
    //  {
    //    string apiResponse = await response.Content.ReadAsStringAsync();
    //    if( JsonConvert.DeserializeObject<List<InvoiceDTO>>( apiResponse ) != null )
    //    {
    //      invoices = JsonConvert.DeserializeObject<List<InvoiceDTO>>( apiResponse );
    //    }
    //  }

    //  List<BillingSystem.Service.Invoice> custInvoices = new List<BillingSystem.Service.Invoice>();
    //  foreach( var item in invoices )
    //  {
    //    BillingSystem.Service.Invoice invoice = new BillingSystem.Service.Invoice();
    //    invoice.Id = default;
    //    invoice.AccountNumber = item.accountNumber;
    //    invoice.OldAccountId = item.oldAccountId;
    //    invoice.GlDate = item.glDate;
    //    invoice.Credit = item.credit;
    //    invoice.Debit = item.debit;
    //    invoice.TrxDate = item.trxDate;
    //    invoice.RefNo = item.refNo;
    //    invoice.TransactionClass = item.transactionClass;
    //    invoice.DocNumber = item.docNumber;
    //    invoice.CustomerPartyId = item.customerPartyId;
    //    invoice.CustTrxTypeId = item.custTrxTypeId;
    //    invoice.AccountName = item.accountName;
    //    invoice.Status = item.status;
    //    invoice.DueDate = item.dueDate;
    //    custInvoices.Add( invoice );
    //  }

    //  BillingSystem.Service.RefreshInvoiceDTO refreshInvoiceDTO = new BillingSystem.Service.RefreshInvoiceDTO();
    //  refreshInvoiceDTO.AccountNumber = _accountNumber;
    //  refreshInvoiceDTO.Invoices = custInvoices;
    //  var json = JsonConvert.SerializeObject( refreshInvoiceDTO );
    //  var content = new StringContent( json, Encoding.UTF8, "application/json" );

    //  bool invoicesResult;
    //  using( var response = await _httpClient.PostAsync( $"Invoice/RefreshCustomerInvoices/", content ) )
    //  {
    //    string apiResponse = await response.Content.ReadAsStringAsync();
    //    invoicesResult = JsonConvert.DeserializeObject<bool>( apiResponse );
    //  }

    //  return RedirectToAction("Index");
    //}
  }
}
