using Bulky.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
	public class TbCompany
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public AddressInfo AddressInfo { get; set; } = null!;
	}
}
