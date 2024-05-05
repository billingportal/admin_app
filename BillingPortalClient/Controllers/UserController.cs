using Microsoft.AspNetCore.Mvc;
using BillingPortalClient.Models;
using Newtonsoft.Json;
using System.Text;

namespace BillingPortalClient.Controllers
{
  public class UserController : BaseController
  {
    public async Task<ActionResult> ManageUsers()
    {
      int _customerAccountId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault( x => x.Type == "accountId" ).Value );
      string _customerAccountName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "accountName" ).Value;
      string _customerAccountNumber = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "customerAccountNumber" ).Value;

      CustomerUserViewModel customerUserViewModel  = new CustomerUserViewModel();

      List<Customer> customerUsers = new List<Customer>();
      using( var response = await _httpClient.GetAsync( $"User/GetCustomerUsers/{_customerAccountId}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        if( JsonConvert.DeserializeObject<List<Customer>>( apiResponse ) != null )
        {
          customerUsers = JsonConvert.DeserializeObject<List<Customer>>( apiResponse );
        }
      }
      
      customerUserViewModel.customerUsers = customerUsers;


      CustomerUserDTO customerUserDTO = new CustomerUserDTO();
      customerUserDTO.accountName = _customerAccountName;
      customerUserDTO.accountNumber = _customerAccountNumber;
      customerUserViewModel.customerUserDTO = customerUserDTO;
      
      return View(customerUserViewModel);
    }

    public async Task<ActionResult> ManageAdminUsers()
    {
      int adminId = Convert.ToInt32( HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminId" ).Value );
      string adminEmail = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminEmail" ).Value;
      string adminRole = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminRole" ).Value;
      string adminFirstName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminFirstName" ).Value;
      string adminLastName = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminLastName" ).Value;
      string adminStatus = HttpContext.User.Claims.FirstOrDefault( x => x.Type == "adminStatus" ).Value;

      AdminUserViewModel adminUserViewModel = new AdminUserViewModel();
      List<BillingSystem.Service.Admin> adminUsers = new List<BillingSystem.Service.Admin>();
      {
        var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
        adminUsers = ( await client.GetAllAdminUserAsync() ).ToList();
      }

      adminUserViewModel.adminUsers = adminUsers;

      List<BillingSystem.Service.Region> regions = new List<BillingSystem.Service.Region>();
      {
        var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
        regions = ( await client.GetAllRegionsAsync() ).ToList();
      }
      adminUserViewModel.regions = regions;

      //List<int> selectedRegions = new List<int>();
      //selectedRegions.Add( 7 );

      //{
      //  var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
      //  adminUserViewModel.selectedRegions = ( await client.GetAdminRegionsByAdminIdAsync(admin) ).ToList();
      //}
      
      return View( adminUserViewModel );
    }

    [HttpPost]
    public async Task<ActionResult> AddCustomerUser(CustomerUserDTO customerUserDTO)
    {
      if(customerUserDTO != null)
      {
        var json = JsonConvert.SerializeObject( customerUserDTO );
        var content = new StringContent( json, Encoding.UTF8, "application/json" );

        bool customerUserResult;
        using( var response = await _httpClient.PostAsync( $"User/SaveCustomerUser/", content ) )
        {
          string apiResponse = await response.Content.ReadAsStringAsync();
          customerUserResult = JsonConvert.DeserializeObject<bool>( apiResponse );
        }
        return RedirectToAction( nameof( ManageUsers ) );
      }
      return View("ManageUsers");
    }

    [HttpPost]
    public async Task<ActionResult> UpdateCustomerUser( CustomerUserDTO customerUserDTO )
    {
      if( customerUserDTO != null )
      {
        var json = JsonConvert.SerializeObject( customerUserDTO );
        var content = new StringContent( json, Encoding.UTF8, "application/json" );

        bool customerUserResult;
        using( var response = await _httpClient.PostAsync( $"User/UpdateCustomerUser/", content ) )
        {
          string apiResponse = await response.Content.ReadAsStringAsync();
          customerUserResult = JsonConvert.DeserializeObject<bool>( apiResponse );
        }
        return RedirectToAction( nameof( ManageUsers ) );
      }
      return View( "ManageUsers" );
    }

    [HttpPost]
    public async Task<ActionResult> AddAdminUser(AdminUserViewModel adminUserViewModel )
    {
      bool saveResult = false;
      BillingSystem.Service.AdminUserDTO adminUserDTO= new BillingSystem.Service.AdminUserDTO();
      adminUserDTO.Id = adminUserViewModel.adminUser.Id;
      adminUserDTO.FirstName = adminUserViewModel.adminUser.FirstName;
      adminUserDTO.LastName = adminUserViewModel.adminUser.LastName;
      adminUserDTO.Email = adminUserViewModel.adminUser.Email;
      adminUserDTO.Password = adminUserViewModel.adminUser.Password;
      adminUserDTO.Role = adminUserViewModel.adminUser.Role;
      adminUserDTO.Status = adminUserViewModel.adminUser.Status != null ? adminUserViewModel.adminUser.Status : "";

            adminUserDTO.AdminId = adminUserDTO.Id;

            adminUserDTO.ViewCustomer = true; // Set default value
            adminUserDTO.ViewInvoice = true; // Set default value
            adminUserDTO.ViewPayment = true; // Set default value
            adminUserDTO.ViewStatement = true; // Set default value
            adminUserDTO.ViewTicket = true; // Set default value
            adminUserDTO.UpdateInvoice = false; // Set default value
            adminUserDTO.UpdatePayment = false; // Set default value
            adminUserDTO.UpdateStatement = false; // Set default value
            adminUserDTO.UpdateCustomer = false; // Set default value
            adminUserDTO.SelectedRegions = adminUserViewModel.selectedRegions ?? new List<int> { 1 };




            var json = JsonConvert.SerializeObject( adminUserDTO );
      var content = new StringContent( json, Encoding.UTF8, "application/json" );
      try
      {
        using( var response = await _httpClient.PostAsync( $"User/SaveAdminUser/", content ) )
        {
          string apiResponse = await response.Content.ReadAsStringAsync();
          saveResult = JsonConvert.DeserializeObject<bool>( apiResponse );
        }
      }
      //try
      //{
      //  var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
      //  saveResult = await client.SaveAdminUserAsync( adminUserDTO ); 
      //}
      catch( Exception ex )
      {
        throw new Exception( ex.Message );
      }
      if(saveResult == true)
      {

      }
      return RedirectToAction( "ManageAdminUsers", "User" );
    }

    [HttpPost]
    public async Task<ActionResult> UpdateAdminPermissions(BillingSystem.Service.AdminPermission adminPermission )
    {
      try
      {


        bool result = false;
        {
          var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
          result = await client.SaveAdminPermissionsAsync( adminPermission );
        }
        return RedirectToAction( "ManageAdminUsers" );
      }
      catch(Exception ex)
      {
        throw new Exception( ex.Message );
      }
    }

    public async Task<ActionResult> SetAdminStatus( int adminId, string status )
    {
      try
      {
        bool result = false;
        {
          var client = new BillingSystem.Service.Client( baseUrl, _httpClient );
          result = await client.SetAdminUserStatusAsync( adminId, status );
        }
      }
      catch(Exception ex)
      {
        throw new Exception( ex.Message );
      }

      return RedirectToAction( "ManageAdminUsers", "User" );
    }
  }
}
