using Microsoft.AspNetCore.Identity;

namespace CapstoneProject.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string ImgPath { get; set; }
    }
}
