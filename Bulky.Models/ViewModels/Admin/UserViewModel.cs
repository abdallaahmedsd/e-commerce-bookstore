using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels.Admin
{
	public class UserListViewModel
	{
        public string Name { get; set; }
		public string Email { get; set; } 
		public string? Phone { get; set; }
		public string? Company { get; set; }
		public string Role { get; set; }
    }
}
