using Microsoft.AspNetCore.Identity;

namespace Bulky.Models.Identity
{
	public class ApplicationRole : IdentityRole<int>
	{
		public ApplicationRole() : base() { }
		public ApplicationRole(string roleName) : base(roleName) { }
	}
}
