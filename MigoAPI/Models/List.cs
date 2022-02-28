using System.ComponentModel.DataAnnotations;

namespace MigoAPI.Models
{
    public class List
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public int ItemsCounter { get; set; } = 0;
    }
}
