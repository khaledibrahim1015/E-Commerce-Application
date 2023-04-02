using Microsoft.AspNetCore.Identity;

namespace E_Project_.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }


    }
}
