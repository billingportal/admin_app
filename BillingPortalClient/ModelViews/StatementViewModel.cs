using BillingPortalClient.Models;

namespace BillingPortalClient.ModelViews
{
  public class StatementViewModel
  {
    public List<StatementRow> statementRows { get; set; }
    public TicketViewModel ticketViewModel { get; set; }

    public List<BillingSystem.Service.Statement> statements { get; set; }
    public int allTransactionCount { get; set; }
    public decimal debitAmountTotal { get; set; }
    public decimal creditAmountTotal { get; set; }
    public string accountName { get; set; }
    public string accountNumber { get; set; }
  }

  public class StatementRow
  {
    public int id { get; set; }
    public string refNo { get; set; }
    public string docNumber { get; set; }
    public DateTime createdDate { get; set; }
    public string type { get; set; }
    public decimal debit { get; set; }
    public decimal credit { get; set; }
    public decimal balance { get; set; }
    public string accountNumber { get; set; }
    public string region { get; set; }
  }
}
