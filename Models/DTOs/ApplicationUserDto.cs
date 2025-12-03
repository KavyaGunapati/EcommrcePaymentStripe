

namespace Models.DTOs
{
        public class Login
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public class Register
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
            public string FullName { get; set; } = null!;
            public string Role { get; set; } = string.Empty;
        }

        public class UpdateUser
        {
            public string FullName { get; set; } = string.Empty;
        }

        public class UserDto
        {
            public string Id { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public List<string> Roles { get; set; } = new();
        }

        public class AuthResponse
        {
            public string AccessToken { get; set; } = null!;
            public string RefreshToken { get; set; } = null!;
        }

        public class RefreshRequest
        {
            public string RefreshToken { get; set; } = null!;
        }
    }




