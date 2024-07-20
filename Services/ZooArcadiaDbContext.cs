using Microsoft.EntityFrameworkCore;
using Image = ZooArcadia.API.Models.DbModels.Image;
using ZooArcadia.API.Models.DbModels;

public class ZooArcadiaDbContext : DbContext
{
    public ZooArcadiaDbContext(DbContextOptions<ZooArcadiaDbContext> options) : base(options)
    {
    }

    public DbSet<Animal> animal { get; set; }
    public DbSet<Avis> avis { get; set; }
    public DbSet<Habitat> habitat { get; set; }
    public DbSet<Image> image { get; set; }
    public DbSet<Race> race { get; set; }
    public DbSet<RapportVeterinaire> rapportveterinaire { get; set; }
    public DbSet<Role> role { get; set; }
    public DbSet<Service> service { get; set; }
    public DbSet<UserZoo> userzoo { get; set; }
    public DbSet<HabitatImageRelation> habitatimagerelation { get; set; }
    public DbSet<AnimalImageRelation> animalimagerelation { get; set; }
    public DbSet<AnimalFeeding> animalfeeding { get; set; }
    public DbSet<Footer> footer { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HabitatImageRelation>()
            .HasKey(h => new { h.habitatid, h.imageid });

        modelBuilder.Entity<AnimalImageRelation>()
            .HasKey(a => new { a.animalid, a.imageid });

        modelBuilder.Entity<Animal>()
            .HasOne(a => a.race)
            .WithMany(r => r.animal)
            .HasForeignKey(a => a.raceid);

        modelBuilder.Entity<Animal>()
            .HasOne(a => a.habitat)
            .WithMany(h => h.animal)
            .HasForeignKey(a => a.habitatid);

        modelBuilder.Entity<AnimalFeeding>()
            .HasOne(af => af.animal)
            .WithMany(a => a.animalfeeding)
            .HasForeignKey(af => af.animalid);

    }


}
