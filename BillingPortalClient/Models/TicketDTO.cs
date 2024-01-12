namespace BillingPortalClient.Models
{
  public class TicketDTO
  {
  }

  public class TicketStatusDTO
  {
    public int TicketId { get; set; }
    public string Status { get; set; }
  }

  public class TicketAssigneeDTO
  {
    public int TicketId { get; set; }
    public int AssigneeId { get; set; }
  }

  public class TicketNotesDTO
    {
    public int AdminId { get;set; }
    public string Notes { get; set; }
    public int TicketId { get; set; }
    
    }
}
