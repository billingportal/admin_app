namespace BillingPortalClient.Models
{
  public class Statement
  {
    public int Id { get; set; }

    public string? TransactionClass { get; set; }

    public string? DocNumber { get; set; }

    public DateTime? GlDate { get; set; }

    public DateTime? TrxDate { get; set; }

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }

    public int? CustomerPartyId { get; set; }

    public int? CustTrxTypeId { get; set; }

    public string? RefNo { get; set; }

    public string? AccountNumber { get; set; }

    public string? AccountName { get; set; }

    public string? OldAccountId { get; set; }

    public int? AccountId { get; set; }

    public int? CustomerId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Customer? Customer { get; set; }
  }
}
