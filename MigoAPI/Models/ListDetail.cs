using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MigoAPI.Models
{
    public class ListDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ListId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [ForeignKey("ListId")]
        public List List { get; set; }

        [ForeignKey("ItemId")]
        public Item Item { get; set; }
    }
}
