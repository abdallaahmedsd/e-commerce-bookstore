using Bulky.Models.Common;
using Microsoft.AspNetCore.Identity;

namespace Bulky.Models.Identity
{
	public class ApplicationUser : IdentityUser<int>
	{
		public string Name { get; set; } = null!;

		public AddressInfo ContactInfo { get; set; } = new();
	}
}
