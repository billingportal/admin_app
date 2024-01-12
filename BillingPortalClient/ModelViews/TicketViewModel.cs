using BillingPortalClient.Models;

namespace BillingPortalClient.ModelViews
{
  public class TicketViewModel
  {
    public string issueType { get; set; }
    public string instrumentType { get; set; }
    public List<string> instrumentNumbers { get; set; }
    public string description { get; set; }
    public string accountNumber { get; set; }
    public string image { get; set; }

    public IFormFile imageFile { get; set; }

    public TicketCommentViewMoodel ticketComment { get; set; }
    public List<BillingSystem.Service.Ticket> tickets { get; set; }
    public BillingSystem.Service.Ticket ticket { get; set; }
    public List<TicketCommentViewMoodel> ticketComments { get; set; }
    public int invoiceTicketsCount { get; set; }
    public int allTicketsCount { get; set; }
    public int statementTicketsCount { get; set; }
    public int receiptTicketsCount { get; set; }
    public int openTicketsCount { get; set; }
    public int closedTicketsCount { get; set; }
    public int inProgressTicketsCount  { get; set; }
    public List<BillingSystem.Service.Admin> admins { get; set; }
    public List<RolesUsers> rolesUsers { get; set; }

  }

  public class TicketCommentViewMoodel
  {
    public int ticketId { get; set; }
    public string comment { get; set; }
    public string accountNumber { get; set; }
    public DateTime createdDate { get; set; }
    public int adminId { get; set; }
  }

  public class InstrumentNumber
  {
    public string value { get; set; }
    public string text { get; set; }
  }

  public class RolesUsers
  {
    public string roleName { get; set; }
    public List<BillingSystem.Service.Admin> users { get; set; }
  }
}
