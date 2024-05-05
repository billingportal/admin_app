using Microsoft.AspNetCore.Mvc;
using BillingPortalClient.Models;
using Newtonsoft.Json;
using System.Text;
using System.Security.Claims;
using System.IO;
using System.Threading.Tasks;

namespace BillingPortalClient.Controllers
{
  public class TicketController : BaseController
  {

    private IWebHostEnvironment Environment;

    public TicketController(IWebHostEnvironment webHostEnvironment)
    {
      Environment = webHostEnvironment;
    }

    public async Task<ActionResult> Tickets()
{
    // Retrieve admin claims from HttpContext
    var adminClaims = HttpContext.User.Claims.ToDictionary(c => c.Type, c => c.Value);

    // Instantiate a new client with the provided base URL and HttpClient
    var client = new BillingSystem.Service.Client(baseUrl, _httpClient);

    // Retrieve all tickets and customers associated with the admin ID
    var allTickets = (await client.GetAllTicketsAsync()).ToList();
    //var customers = (await client.GetCustomersByAdminIdAsync(Convert.ToInt32(adminClaims["adminId"]))).ToList();
    Console.WriteLine("All Tickets Get Successfully");
    // Filter tickets based on customer IDs associated with the admin
    //var customerIds = customers.Select(c => c.Id).ToList();
    var tickets = allTickets; //.Where(t => customerIds.Contains((int)t.CustomerId)).ToList();

    // Create a view model to pass ticket-related data to the view
    var ticketViewModel = new TicketViewModel
    {
        tickets = tickets,
        allTicketsCount = tickets.Count,
        invoiceTicketsCount = tickets.Count(t => t.InstumentType == "Invoice"),
        statementTicketsCount = tickets.Count(t => t.InstumentType == "Statement"),
        receiptTicketsCount = tickets.Count(t => t.InstumentType == "Receipt"),
        openTicketsCount = tickets.Count(t => t.Status == "Open"),
        closedTicketsCount = tickets.Count(t => t.Status == "Closed"),
        inProgressTicketsCount = tickets.Count(t => t.Status == "Inprogress")
    };

    // Return the view along with the populated view model
    return View(ticketViewModel);
}



   [HttpPost]
public async Task<ActionResult> SaveTicket(TicketViewModel ticketViewModel)
{
    string imagePath = ""; // Initialize imagePath

    if (ticketViewModel.imageFile != null && ticketViewModel.imageFile.Length > 0)
    {
        // Generate unique file name for image
        string uniqueFileName = Guid.NewGuid() + "_" + ticketViewModel.imageFile.FileName;
        imagePath = "/appImages/" + uniqueFileName;

        // Save image file to wwwroot
        imagePath = Path.Combine(this.Environment.WebRootPath, imagePath);
        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await ticketViewModel.imageFile.CopyToAsync(stream);
        }
    }

    // Set account number and image path in view model
    ticketViewModel.accountNumber = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "customerAccountNumber")?.Value;
    ticketViewModel.image = imagePath;

    // Serialize view model to JSON
    var json = JsonConvert.SerializeObject(ticketViewModel);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    // Post data to API endpoint
    bool ticketResult;
    using (var response = await _httpClient.PostAsync("Ticket/SaveTicket/", content))
    {
        string apiResponse = await response.Content.ReadAsStringAsync();
        ticketResult = JsonConvert.DeserializeObject<bool>(apiResponse);
    }

    return RedirectToAction("Tickets");
}


    public async Task<ActionResult> TicketDetail(int ticketId)
{
    // Retrieve ticket details from API
    BillingSystem.Service.Ticket ticket;
    using (var response = await _httpClient.GetAsync($"Ticket/GetTicketById/{ticketId}"))
    {
        string apiResponse = await response.Content.ReadAsStringAsync();
        ticket = JsonConvert.DeserializeObject<BillingSystem.Service.Ticket>(apiResponse);
    }

    TicketViewModel ticketViewModel = new TicketViewModel();
    ticketViewModel.ticket = ticket;

    try
    {
        // Get all admin users
        var client = new BillingSystem.Service.Client(baseUrl, _httpClient);
        var adminUsers = (await client.GetAllUsersAsync()).ToList();

        // Group admin users by regional and non-regional roles
        var rolesUsers = adminUsers.GroupBy(
            admin => admin.AdminRegions.Any(region => region.Region.Region1 == ticket.Account.Customer.Region),
            (isRegional, admins) => new RolesUsers
            {
                roleName = isRegional ? ticket.Account.Customer.Region : "Non Regional",
                users = admins.ToList()
            }).ToList();

        ticketViewModel.rolesUsers = rolesUsers;
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }

    return View(ticketViewModel);
}


    [HttpPost]
    public async Task<ActionResult> AddTicketComment(TicketCommentViewMoodel ticketComment)
    {
      //ClaimsPrincipal claimsUser = HttpContext.User;
      //string _accountNumber = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "customerAccountNumber" ).Value;
      //ticketComment.accountNumber = _accountNumber;
      int adminId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminId" ).Value );
      ticketComment.adminId= adminId;

      var json = JsonConvert.SerializeObject( ticketComment );
      var content = new StringContent( json, Encoding.UTF8, "application/json" );

      bool ticketResult;
      using( var response = await _httpClient.PostAsync( $"Ticket/SaveTicketComment/", content ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        ticketResult = JsonConvert.DeserializeObject<bool>( apiResponse );
      }

      if(ticketResult)
      {

      }

      return RedirectToAction( "TicketDetail", new { ticketId = ticketComment.ticketId} );
    }

    [HttpPost]
    public async Task<ActionResult> UpdateTicketStatus(int ticketId, string status)
    {
      bool responseResult = false;
      {
        var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
        responseResult = await client.UpdateTicketStatusAsync( ticketId, status );
      }
      return RedirectToAction( "TicketDetail", "Ticket", new {ticketId = ticketId} );
    }

    [HttpPost]
    public async Task<ActionResult<bool>> UpdateTicketStatus1([FromBody] TicketStatusDTO ticketStatusDTO )
    {
      try
      {
        bool responseResult = false;
        {
          var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
          responseResult = await client.UpdateTicketStatusAsync( ticketStatusDTO.TicketId, ticketStatusDTO.Status );
        }

        return true;
      }
      catch (Exception ex)
      {
        throw new Exception( ex.Message );
      }
    }

    [HttpPost]
    public async Task<ActionResult<BillingSystem.Service.Ticket>> UpdateTicketAssignee( [FromBody] TicketAssigneeDTO ticketAssigneeDTO )
    {
      try
      {
        BillingSystem.Service.Ticket responseResult = new BillingSystem.Service.Ticket();
        {
          var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
          responseResult = await client.UpdateTicketAssigneeAsync( ticketAssigneeDTO.TicketId, ticketAssigneeDTO.AssigneeId );
        }

        return responseResult;
      }
      catch( Exception ex )
      {
        throw new Exception( ex.Message );
      }
    }

    [HttpPost]
    public async Task<ActionResult<BillingSystem.Service.TicketNote>> AddTicketNotes( [FromBody] TicketNotesDTO ticketNotesDTO )
    {
      try
      {

        BillingSystem.Service.TicketNote ticketNote = new BillingSystem.Service.TicketNote();
        {
          var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
          ticketNote = await client.SaveTicketNotesAsync( (int)ticketNotesDTO.AdminId, ticketNotesDTO.Notes, ticketNotesDTO.TicketId );
        }

        return ticketNote;
      }
      catch( Exception ex )
      {
        throw new Exception( ex.Message );
      }
    }
  }
}
