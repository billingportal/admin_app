using BillingPortalClient.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using static BillingPortalClient.ModelViews.CustomerSelModelViews;


namespace BillingPortalClient.Services
{
  public class OracleApiServices
  {
    Uri oracleBaseAddress = new Uri( "http://88.85.242.29/api/" );
    protected readonly HttpClient _httpClientOracle;
    public OracleApiServices()
    {
      _httpClientOracle = new HttpClient();
      _httpClientOracle.BaseAddress = oracleBaseAddress;
    }
    public async Task<List<InvoiceDTO>> GetCustomerInvoicesFromOracle(string accountNumber)
    {
      List<InvoiceDTO> invoices = new List<InvoiceDTO>();
      using( var response = await _httpClientOracle.GetAsync( $"Invoice/GetCustomerInvoices/{accountNumber}" ) )
      {
        string apiResponse = await response.Content.ReadAsStringAsync();
        if( JsonConvert.DeserializeObject<List<InvoiceDTO>>( apiResponse ) != null )
        {
          invoices = JsonConvert.DeserializeObject<List<InvoiceDTO>>( apiResponse );
        }
      }

      return invoices;
    }


        public async Task<List<CustomerList>> GetAllCustomers()
        {
            try
            {
                HttpResponseMessage response = await _httpClientOracle.GetAsync("CustomerOracle/GetAllCustomers/");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Response from the API");
                    var customers = await response.Content.ReadAsAsync<List<CustomerList>>();
                    return customers;
                }

                // Handle specific HTTP error cases
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // Handle 404 Not Found
                    Console.WriteLine("API endpoint not found.");
                }
                else
                {
                    // Handle other HTTP error cases
                    Console.WriteLine($"Error fetching customers. Status code: {response.StatusCode}");
                }

                // Handle other exceptions (e.g., HttpRequestException, JsonException) as needed
                throw new Exception($"Error fetching customers. Status code: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exception
                Console.WriteLine($"HTTP request exception: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                // Handle JSON parsing exception
                Console.WriteLine($"JSON parsing exception: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<CustomerList>> GetAllCustomersByAdminId(int adminId)
        {
            HttpResponseMessage response = await _httpClientOracle.GetAsync($"CustomerOracle/GetAllCustomersByAdminId/{adminId}");

            if (response.IsSuccessStatusCode)
            {
                var customers = await response.Content.ReadAsAsync<List<CustomerList>>();
                return customers;
            }

            // Handle error cases
            throw new Exception($"Error fetching customers. Status code: {response.StatusCode}");
        }

        public async Task<List<CustomerList>> GetCustomersByLocation(string location)
        {
             try
            {
                Console.WriteLine("Request in Services for: {location}");
            HttpResponseMessage response = await _httpClientOracle.GetAsync($"CustomerOracle/GetCustomersByLocationFromOracle/{location}");

            if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Response from the API");
                    var customers = await response.Content.ReadAsAsync<List<CustomerList>>();
                    return customers;
                }

                // Handle specific HTTP error cases
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // Handle 404 Not Found
                    Console.WriteLine("API endpoint not found.");
                }
                else
                {
                    // Handle other HTTP error cases
                    Console.WriteLine($"Error fetching customers. Status code: {response.StatusCode}");
                }

                // Handle other exceptions (e.g., HttpRequestException, JsonException) as needed
                throw new Exception($"Error fetching customers. Status code: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exception
                Console.WriteLine($"HTTP request exception: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                // Handle JSON parsing exception
                Console.WriteLine($"JSON parsing exception: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }


        public async Task<List<EmailList>> GetEmailsByCustomer(string accountNumber)
        {
            HttpResponseMessage response = await _httpClientOracle.GetAsync($"CustomerOracle/GetEmailsByCustomer/{accountNumber}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<EmailList>>();
            }

            // Handle error cases
            throw new Exception($"Error fetching emails. Status code: {response.StatusCode}");
        }

        public async Task<List<AccountList>> GetAccountsByEmail(string email)
        {
            HttpResponseMessage response = await _httpClientOracle.GetAsync($"CustomerOracle/GetAccountsByEmail/{email}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<AccountList>>();
            }

            // Handle error cases
            throw new Exception($"Error fetching accounts. Status code: {response.StatusCode}");
        }

        

        // Add other methods as needed

        public void Dispose()
        {
            _httpClientOracle.Dispose();
        }


    }
}
