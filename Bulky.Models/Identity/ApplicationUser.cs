using Microsoft.AspNetCore.Identity;

namespace Bulky.Models.Identity
{
	public class ApplicationUser : IdentityUser<int>
	{
		public string Name { get; set; } = null!;

        public string? StreetAddress { get; set; }

        public string? City { get; set; }
        
        public string? State { get; set; }
        
        public string? PostalCode { get; set; }
    }
}
