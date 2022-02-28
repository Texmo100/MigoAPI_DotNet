using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MigoAPI.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Episodes { get; set; } = 0;
        public string Genre { get; set; }
        public double Rating { get; set; } = 0;
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        [Required]
        public int TypeId { get; set; }

        [ForeignKey("TypeId")]
        public Type Type { get; set; }

    }
}
