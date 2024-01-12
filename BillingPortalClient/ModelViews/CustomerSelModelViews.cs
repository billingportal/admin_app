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
            public int Id { get; set; }
            public string Name { get; set; }
            public List<string> Emails { get; set; }
            public string PhoneNumber { get; set; }

            public string Location { get; set; }
            // Add other properties related to Customer
        }

        public class EmailList
        {
            public int Id { get; set; }
            public string Address { get; set; }
            // Add other properties related to Email
        }

        public class AccountList
        {
            public int AccountId { get; set; } // Rename to avoid conflict
            public string Number { get; set; }
            // Add other properties related to Account
        }
    }
}
