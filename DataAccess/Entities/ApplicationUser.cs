using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{

    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}
