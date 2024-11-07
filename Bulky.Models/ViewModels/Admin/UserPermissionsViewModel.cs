using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bulky.Models.ViewModels.Admin
{
	public class UserPermissionsViewModel
	{
		public int Id { get; set; }

		[ValidateNever]
		public string Name { get; set; }
		public string Role { get; set; }

		[DisplayName("Company")]
		public int? CompanyId { get; set; }

		[ValidateNever]
		public List<SelectListItem> Roles { get; set; }

		[ValidateNever]
		public List<CompanyForPermissionsViewModel> Companies { get; set; }
	}

	public class RoleViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class CompanyForPermissionsViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}


}
