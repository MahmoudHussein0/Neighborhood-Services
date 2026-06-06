namespace Neighborhood.Services.Infrastructure.Services.Payments.Paymob
{
    public class PaymobCreateOrderRequest
    {
        public string Auth_Token { get; set; } = string.Empty;
        public int Delivery_Needed { get; set; } = 0;
        public int Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string Merchant_Order_Id { get; set; } = string.Empty;
        public List<object> Items { get; set; } = new();
    }
}
