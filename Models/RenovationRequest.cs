using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LarmoireWeb.Models
{
    public enum RequestStatus
    {
        Pending,
        Accepted,
        Rejected,    // ← ajouté
        Completed
    }

    public class RenovationRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Required, StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public RequestStatus Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [StringLength(2000)]
        public string ImageUrls { get; set; }  // ← nouveau champ
    }
}
