using Microsoft.EntityFrameworkCore;

namespace SistemaAdopcion.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<Adopter> Adopters { get; set; }
        public DbSet<Adoption> Adoptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Restricción: Una mascota solo puede ser adoptada una vez
            modelBuilder.Entity<Adoption>()
                .HasIndex(a => a.PetId)
                .IsUnique();

            // Configuración de relaciones
            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.Pet)
                .WithOne(p => p.Adoption)
                .HasForeignKey<Adoption>(a => a.PetId);

            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.Adopter)
                .WithOne(ad => ad.Adoption)
                .HasForeignKey<Adoption>(a => a.AdopterId);
        }
    }
}