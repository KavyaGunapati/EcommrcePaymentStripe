using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class RefreshTokenDto
    {
        [Required]
        public string Token { get; set; } = null!;
       
    }
}
