//using BillingPortalClient.Models;
using BillingSystem.Service;

namespace BillingPortalClient.ModelViews
{
  public class InvoiceViewModel
  {
    public List<Invoice> invoices { get; set; }
    public List<string> selectedInvoiceNumbers   { get; set; }
    public string paymentMethod { get; set; }
    public int paymentAmount { get; set; }
    public PaymentViewModel PaymentViewModel { get; set; }
    public List<InvoiceRow> invoiceTable { get; set; }
    public TicketViewModel ticketViewModel { get; set; }
    public List<Invoice> disputedInvoices { get; set; }
    public string accountName { get; set; }
    public string accountNumber { get; set; }
    public List<InvoiceRowDTO> invoiceRowDTOs { get; set; }
    public double openTransactions { get; set; }
    public double overdueAmount { get; set; }
  }
}
