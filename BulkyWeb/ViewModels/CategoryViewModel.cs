using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWeb.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [DisplayName("Display Order")]
        [Range(1, 100)]
        public int DisplayOrder { get; set; }
    }
}
