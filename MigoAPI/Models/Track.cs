using System;
using System.ComponentModel.DataAnnotations;

namespace MigoAPI.Models
{
    public class Track
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int EpisodesWatched { get; set; }
    }
}
