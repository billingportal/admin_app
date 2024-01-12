using BillingPortalClient.Models;
using Newtonsoft.Json;
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


        public async Task<List<CustomerList>> GetCustomersByLocation(string location)
        {
            HttpResponseMessage response = await _httpClientOracle.GetAsync($"GetCustomersByLocation/{location}");

            if (response.IsSuccessStatusCode)
            {
                var customers = await response.Content.ReadAsAsync<List<CustomerList>>();
                return customers;
            }

            // Handle error cases
            throw new Exception($"Error fetching customers. Status code: {response.StatusCode}");
        }


        public async Task<List<EmailList>> GetEmailsByCustomer(int customerId)
        {
            HttpResponseMessage response = await _httpClientOracle.GetAsync($"GetEmailsByCustomer/{customerId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<EmailList>>();
            }

            // Handle error cases
            throw new Exception($"Error fetching emails. Status code: {response.StatusCode}");
        }

        public async Task<List<AccountList>> GetAccountsByEmail(string emailId)
        {
            HttpResponseMessage response = await _httpClientOracle.GetAsync($"GetAccountsByEmail/{emailId}");

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
