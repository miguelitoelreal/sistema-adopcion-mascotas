using System.ComponentModel.DataAnnotations;

namespace SistemaAdopcion.Models
{
    public class Adopter
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public Adoption? Adoption { get; set; } // Relación 1:1 con Adoption
    }
}