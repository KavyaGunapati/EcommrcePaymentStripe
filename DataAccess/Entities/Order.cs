namespace DataAccess.Entities
{


    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!; // Pending, Paid, Failed
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Payment Payment { get; set; } = null!;
    }

}
