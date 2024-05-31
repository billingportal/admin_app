using BillingPortalClient.Models;
using BillingPortalClient.Services;

namespace BillingPortalClient.ModelViews
{
  public class CustomerInvoiceViewModel
  {
    public List<CustomerInvoice> invoices { get; set; }
        public List<string> selectedInvoiceNumbers { get; set; }
        public string paymentMethod { get; set; }
        public double paymentAmount { get; set; }
        public PaymentViewModel PaymentViewModel { get; set; }
        public List<InvoiceRow> invoiceTable { get; set; }
        public TicketViewModel ticketViewModel { get; set; }
        public List<CustomerInvoice> disputedInvoices { get; set; }
        public string accountName { get; set; }
        public string arabicName { get; set; }
        public string accountNumber { get; set; }
        public string newAccountNumber { get; set; }
        public string businessUnitId { get; set; }
        public double openTransaction { get; set; }
        public double overdueAmount { get; set; }
        public string invoiceStatus { get; set; } // Updated property name
  }
}
