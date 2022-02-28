using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MigoAPI.Models
{
    public class TrackDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TrackId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [ForeignKey("TrackId")]
        public Track Track { get; set; }

        [ForeignKey("ItemId")]
        public Item Item { get; set; }
    }
}
