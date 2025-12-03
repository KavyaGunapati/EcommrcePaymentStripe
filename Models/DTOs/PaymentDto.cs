namespace Models.DTOs
{

    public class PaymentDto
    {
        public int OrderId { get; set; }
        public string StripePaymentIntentId { get; set; }
        public string Status { get; set; }
    }

}
