using System;
using System.Collections.Generic;

namespace BillingPortalClient.Models;

public partial class TicketComment
{
    public int Id { get; set; }

    public int? TicketId { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CustomerAccountId { get; set; }

    public int? AdminId { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Account? CustomerAccount { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
