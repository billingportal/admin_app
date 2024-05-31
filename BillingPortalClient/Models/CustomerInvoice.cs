using System;
using System.Collections.Generic;
using BillingPortalClient.Models;
using BillingPortalClient.ModelViews;

namespace BillingPortalClient.Models
{
    public partial class CustomerInvoice
    {
        public string TransactionNumber { get; set; }
        public string TransactionType { get; set; }
        public string InvoiceCurrencyCode { get; set; }
        public decimal? EnteredAmount { get; set; }
        public decimal? TotalPaidAmount { get; set; }
        public decimal? InvoiceBalanceAmount { get; set; }
        public string InvoiceStatus { get; set; }
        public string InvoicePaymentStatus { get; set; }
        public string BusinessUnit { get; set; }
        public string BusinessUnitID { get; set; }
        public int? CustomerTransactionId { get; set; }
        public DateTime DueDate { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionSource { get; set; }
        public string BillToCustomerNumber { get; set; }
        public string BillToSite { get; set; }
        public string BillToPartyId { get; set; }

        public int? CustomerId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<InvoicesPayment> InvoicesPayments { get; set; } = new List<InvoicesPayment>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}