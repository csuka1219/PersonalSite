using Microsoft.AspNetCore.Identity;

namespace PersonalSite.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
    }

}
