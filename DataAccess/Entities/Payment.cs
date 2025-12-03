namespace DataAccess.Entities
{

    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string StripePaymentIntentId { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

}
