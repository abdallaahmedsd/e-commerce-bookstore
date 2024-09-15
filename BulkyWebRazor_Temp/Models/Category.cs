using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWebRazor_Temp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [DisplayName("Display Order")]
        [Range(1, 100)]
        public int DisplayOrder { get; set; }
    }
}
