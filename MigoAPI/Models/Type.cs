using System.ComponentModel.DataAnnotations;

namespace MigoAPI.Models
{
    public class Type
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
