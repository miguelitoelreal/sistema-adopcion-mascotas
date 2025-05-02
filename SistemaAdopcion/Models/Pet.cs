using System.ComponentModel.DataAnnotations;

namespace SistemaAdopcion.Models
{
    public class Pet
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Range(0, 100)]
        public int Age { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required]
        public string AdoptionStatus { get; set; } // Disponible o Adoptada

        public Adoption? Adoption { get; set; } // Relación 1:1 con Adoption
    }
}