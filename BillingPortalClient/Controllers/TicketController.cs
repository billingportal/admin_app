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
    //Uri baseAddress = new Uri( "https://localhost:7069/api/" );
    ////Uri baseAddress = new Uri( "https://billingportalapis.azurewebsites.net/" );

    //private readonly HttpClient _httpClient;

    //public TicketController()
    //{
    //  _httpClient = new HttpClient();
    //  _httpClient.BaseAddress = baseAddress;
    //}

    private IWebHostEnvironment Environment;

    public TicketController(IWebHostEnvironment webHostEnvironment)
    {
      Environment = webHostEnvironment;
    }

    public async Task<ActionResult> Tickets()
    {
      //ClaimsPrincipal claimsUser = HttpContext.User;
      //string _accountNumber = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "customerAccountNumber" ).Value;
      int adminId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminId" ).Value );
      string adminEmail = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminEmail" ).Value;
      string adminRole = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminRole" ).Value;
      string adminFirstName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminFirstName" ).Value;
      string adminLastName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminLastName" ).Value;
      string adminStatus = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminStatus" ).Value;

      //List<Ticket> tickets = new List<Ticket>();
      //using( var response = await _httpClient.GetAsync( $"Ticket/GetTicketsByAccountNumber/{_accountNumber}" ) )
      //{
      //  string apiResponse = await response.Content.ReadAsStringAsync();
      //  tickets = JsonConvert.DeserializeObject<List<Ticket>>( apiResponse );
      //}
      List<BillingSystem.Service.Ticket> allTickets = new List<BillingSystem.Service.Ticket>();
      {
        var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
        Console.WriteLine("Getting all Tickets");
        allTickets = ( await client.GetAllTicketsAsync() ).ToList();
      }
      List<BillingSystem.Service.Customer> customers = new List<BillingSystem.Service.Customer>();
      {
        var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
        customers = ( await client.GetCustomersByAdminIdAsync( adminId ) ).ToList();
      }
      List<int> customerIds = customers.Select( x => x.Id ).ToList();
      List<BillingSystem.Service.Ticket> tickets = new List<BillingSystem.Service.Ticket>();

      foreach(var item in allTickets)
      {
        if(customerIds.Contains((int)item.CustomerId))
        {
          tickets.Add( item );
        }
      }

      TicketViewModel ticketViewModel = new TicketViewModel();
      ticketViewModel.tickets = tickets;

      int allTicketsCount = 0;
      int invoiceTicketsCount = 0;
      int statementTicketsCount = 0;
      int receiptTicketsCount = 0;
      int openTicketsCount = 0;
      int closedTicketsCount = 0;
      int inProgressTicketsCount = 0;
      foreach(var item in tickets)
      {
        allTicketsCount = allTicketsCount + 1;
        if(item.InstumentType == "Invoice")
        {
          invoiceTicketsCount = invoiceTicketsCount + 1;
        }
        else if(item.InstumentType == "Receipt")
        {
          receiptTicketsCount = receiptTicketsCount + 1;
        }
        else if(item.InstumentType == "Statement")
        {
          statementTicketsCount = statementTicketsCount + 1; 
        }

        if(item.Status == "Open")
        {
          openTicketsCount = openTicketsCount + 1;
        }
        else if(item.Status == "Closed")
        {
          closedTicketsCount = closedTicketsCount + 1;
        }
        else if(item.Status == "InProgress")
        {
          inProgressTicketsCount = inProgressTicketsCount + 1;
        }
      }

      ticketViewModel.allTicketsCount = allTicketsCount;
      ticketViewModel.invoiceTicketsCount= invoiceTicketsCount;
      ticketViewModel.statementTicketsCount= statementTicketsCount;
      ticketViewModel.receiptTicketsCount = receiptTicketsCount;
      ticketViewModel.openTicketsCount = openTicketsCount;
      ticketViewModel.closedTicketsCount = closedTicketsCount;
      ticketViewModel.inProgressTicketsCount= inProgressTicketsCount;


      return View(ticketViewModel);
    }

    [HttpPost]
    public async Task<ActionResult> SaveTicket(TicketViewModel ticketViewModel)
    {

      string uniqueFileName = null;  //to contain the filename
      //if( file != null )  //handle iformfile
      //{
      //  string rootPath = this.Environment.WebRootPath;
      //  string uploadsFolder = Path.Combine( rootPath, "appImages" );
      //  uniqueFileName = file.FileName;
      //  string filePath = Path.Combine( uploadsFolder, uniqueFileName );
      //  using( var fileStream = new FileStream( filePath, FileMode.Create ) )
      //  {
      //    file.CopyTo( fileStream );
      //  }
      //}

      //propertyDetails.Images = uniqueFileName;
      var imagePath1 = "";

      if( ticketViewModel.imageFile != null && ticketViewModel.imageFile.Length > 0 )
      {
        imagePath1 = "/appImages/" + Guid.NewGuid() + "_" + ticketViewModel.imageFile.FileName;
        //var filePath = Path.Combine( Directory.GetCurrentDirectory(), "wwwroot", imagePath );


        var imagePath = Path.Combine( this.Environment.WebRootPath,  imagePath1 );

        //using( var stream = new FileStream( imagePath, FileMode.Create ) )
        //{
        //  await ticketViewModel.imageFile.CopyToAsync( stream );
        //}

        // Save imagePath to database or perform other operations
      }



      ClaimsPrincipal claimsUser = HttpContext.User;
      string _accountNumber = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "customerAccountNumber" ).Value;
      ticketViewModel.accountNumber = _accountNumber;
      ticketViewModel.image = imagePath1;
      var json = JsonConvert.SerializeObject( ticketViewModel );
      var content = new StringContent( json, Encoding.UTF8, "application/json" );

      bool ticketResult;
      using( var response = await _httpClient.PostAsync( $"Ticket/SaveTicket/", content ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        ticketResult = JsonConvert.DeserializeObject<bool>( apiResponse );
      }

      return RedirectToAction( "Tickets" );

    }

    public async Task<ActionResult> TicketDetail(int ticketId)
    {
      BillingSystem.Service.Ticket ticket = new BillingSystem.Service.Ticket();
      using( var response = await _httpClient.GetAsync( $"Ticket/GetTicketById/{ticketId}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        ticket = JsonConvert.DeserializeObject<BillingSystem.Service.Ticket>( apiResponse );
      }

      TicketViewModel ticketViewModel = new TicketViewModel();
      ticketViewModel.ticket = ticket;

      try
      {
        List<BillingSystem.Service.Admin> adminUsers = new List<BillingSystem.Service.Admin>();
        var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
        adminUsers = ( await client.GetAllUsersAsync() ).ToList();
        
        List<RolesUsers> rolesUsers = new List<RolesUsers>();
        string ticketRegion = ticket.Account.Customer.Region;

        foreach(var item in adminUsers)
        {
          bool isAdminRegional = false;
          foreach(var adminRegion in item.AdminRegions)
          {
            if(adminRegion.Region.Region1 == ticketRegion)
            {
              isAdminRegional = true;
              break;
            }
          }

          if( isAdminRegional )
          {
            RolesUsers ru = rolesUsers.FirstOrDefault( x => x.roleName == ticketRegion );
            if( ru == null )
            {
              RolesUsers newRoleUser = new RolesUsers();
              newRoleUser.roleName = ticketRegion;
              newRoleUser.users = new List<BillingSystem.Service.Admin>();
              newRoleUser.users.Add( item );
              rolesUsers.Add( newRoleUser );
            }
            else
            {
              ru.users.Add(item);
            }
          }
          else
          {
            RolesUsers ru = rolesUsers.FirstOrDefault( x => x.roleName == "Non Regional" );
            if( ru == null )
            {
              RolesUsers newRoleUser = new RolesUsers();
              newRoleUser.roleName = ticketRegion;
              newRoleUser.users = new List<BillingSystem.Service.Admin>();
              newRoleUser.users.Add( item );
              rolesUsers.Add( newRoleUser );
            }
            else
            {
              ru.users.Add( item );
            }
          }
        }


        //foreach(var item in adminUsers)
        //{
        //  RolesUsers ru = rolesUsers.FirstOrDefault( x => x.roleName == item.Role );
        //  if(ru == null)
        //  {
        //    RolesUsers newRoleUsers = new RolesUsers();
        //    newRoleUsers.roleName = item.Role;
        //    newRoleUsers.users = new List<BillingSystem.Service.Admin>();
        //    newRoleUsers.users.Add( item );
        //    rolesUsers.Add( newRoleUsers );
        //  }
        //  else
        //  {
        //    ru.users.Add(item);
        //  }
        //}

        ticketViewModel.rolesUsers = rolesUsers;
      }
      catch( Exception ex )
      {
        throw new Exception(ex.Message );
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
