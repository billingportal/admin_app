// BillingPortalClient.ModelViews.CustomerSelModelViews.cs

namespace BillingPortalClient.ModelViews
{
    public class CustomerSelModelViews
    {
        public List<EmailList> Emails { get; set; } 
        public List<AccountList> Accounts { get; set; }
        public List<CustomerList> Customers { get; set; }

        public class CustomerList
        {
             public string accountName { get; set; }
            public string accountNumber { get; set; }
            public string email { get; set; }
            public string phoneNumber { get; set; }
            public string courierRoute { get; set; }
            public string region { get; set; }
            public string city { get; set; }
            public List<AccountList> Accounts { get; set; }
            // Add other properties related to Customer
        }

        public class EmailList
        {
            public int Id { get; set; }
            public string email { get; set; }
            // Add other properties related to Email
        }

        public class AccountList
        {
            public int Id { get; set; } // Rename to avoid conflict
            public string accountNumber { get; set; }
            public string accountName { get; set; }
            public string email { get; set; }
            public string phoneNumber { get; set; }
            public string region { get; set; }
            // Add other properties related to Account
        }
    }
}
