using Microsoft.AspNetCore.Identity;

namespace LinkTree.Models
{
    public class AppUser:IdentityUser<int>
    {
        public string? UserAvatar { get; set; }

        public string? UserBackgroundColor { get; set;}

        public ICollection<Link> Links { get; set; }
    }
}
