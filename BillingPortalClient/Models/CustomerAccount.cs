namespace BillingPortalClient.Models
{

public partial class CustomerAccount
{
    public int? Id { get; set; }
    public int? CustomerId { get; set; }
    public string? AccountNumber { get; set; }
    public string? NewAccountNumber { get; set; }
    public string? AccountName { get; set; }
    public string? ArabicName { get; set; }
    public string? BusinessUnitId { get; set; }
    public bool? IsSelected { get; set; }
    public string? Email { get; set; }
    public string? City { get; set; }
    public string? SalesRegion { get; set; }
    public string? Status { get; set; }
}
}