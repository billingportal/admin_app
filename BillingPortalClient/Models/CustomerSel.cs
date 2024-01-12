// BillingPortalClient.Models.CustomerSel.cs

namespace BillingPortalClient.Models
{
    public class CustomerSel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Location { get; set; }
        // Add other properties related to CustomerSel

        // Navigation properties if needed
        public virtual ICollection<AccountSel> Accounts { get; set; }
        public virtual ICollection<EmailSel> Emails { get; set; }
    }

    public class EmailSel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        // Add other properties related to EmailSel
    }

    public class AccountSel
    {
        public int Id { get; set; }
        public string Number { get; set; }
        // Add other properties related to AccountSel
    }
}
