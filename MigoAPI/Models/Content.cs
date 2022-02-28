using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MigoAPI.Models
{
    public class Content
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ListId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("ListId")]
        public List List { get; set; }
    }
}
