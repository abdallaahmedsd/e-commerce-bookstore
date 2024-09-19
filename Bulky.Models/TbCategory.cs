using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models
{
    public class TbCategory
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

		public int DisplayOrder { get; set; }

        public ICollection<TbBook> Books { get; set; } = [];
    }
}
