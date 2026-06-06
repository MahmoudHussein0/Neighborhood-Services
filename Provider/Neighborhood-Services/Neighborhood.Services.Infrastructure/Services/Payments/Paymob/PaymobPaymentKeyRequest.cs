namespace Neighborhood.Services.Infrastructure.Services.Payments.Paymob
{
    public class PaymobPaymentKeyRequest
    {
        public string Auth_Token { get; set; } = string.Empty;
        public int Amount { get; set; }
        public int Expiration { get; set; } = 3600;
        public int Order_Id { get; set; }
        public string Currency { get; set; } = "EGP";
        public int Integration_Id { get; set; }
        public PaymobBillingData Billing_Data { get; set; } = new();
    }

    public class PaymobBillingData
    {
        public string First_Name { get; set; } = "Customer";
        public string Last_Name { get; set; } = "User";
        public string Email { get; set; } = "customer@example.com";
        public string Phone_Number { get; set; } = "+201000000000";
        public string Apartment { get; set; } = "NA";
        public string Floor { get; set; } = "NA";
        public string Street { get; set; } = "NA";
        public string Building { get; set; } = "NA";
        public string Shipping_Method { get; set; } = "NA";
        public string Postal_Code { get; set; } = "00000";
        public string City { get; set; } = "Cairo";
        public string Country { get; set; } = "EG";
        public string State { get; set; } = "Cairo";
    }
}
