using System;
using System.Collections.Generic;


namespace BillingPortalClient.Models;

public partial class Admin
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<TicketComment> TicketComments { get; set; } = new List<TicketComment>();
}
