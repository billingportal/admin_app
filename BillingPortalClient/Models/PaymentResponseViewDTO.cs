namespace BillingPortalClient.Models
{
  public class PaymentResponseViewDTO
  {
    public string? command { get; set; }
    public string? access_code { get; set; }
    public string? merchant_identifier { get; set; }
    public string? merchant_reference { get; set; }
    public decimal? amount { get; set; }
    public string? currency { get; set; }
    public string? language { get; set; }
    public string? customer_email { get; set; }
    public string? signature { get; set; }
    public string? token_name { get; set; }
    public int? fort_id { get; set; }
    public string? payment_option { get; set; }
    public string? sadad_olp { get; set; }
    public string? knet_ref_number { get; set; }
    public string? third_party_transaction_number { get; set; }
    public string? eci { get; set; }
    public string? order_description { get; set; }
    public string? customer_ip { get; set; }
    public string? customer_name { get; set; }
    public string? merchant_extra { get; set; }
    public string? merchant_extra1 { get; set; }
    public string? merchant_extra2 { get; set; }
    public string? merchant_extra3 { get; set; }
    public string? merchant_extra4 { get; set; }
    public string? merchant_extra5 { get; set; }
    public string? authorization_code { get; set; }
    public string? response_message { get; set; }
    public int? response_code { get; set; }
    public int? status { get; set; }
    public string? card_holder_name { get; set; }
    public int? expiry_date { get; set; }
    public string? card_number { get; set; }
    public string? remember_me { get; set; }
    public string? phone_number { get; set; }
    public string? settlement_reference { get; set; }
    public string? agreement_id { get; set; }
  }
}
