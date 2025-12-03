using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; }=string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

}
