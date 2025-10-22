using Microsoft.EntityFrameworkCore;
using MovieHubMVC.Models;

namespace MovieHubMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<MovieImage> MovieImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Cinema)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CinemaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Actors)
                .WithMany(a => a.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieActor",
                    r => r.HasOne<Actor>().WithMany().HasForeignKey("ActorId"),
                    l => l.HasOne<Movie>().WithMany().HasForeignKey("MovieId"),
                    j =>
                    {
                        j.HasKey("MovieId", "ActorId");
                        j.ToTable("MovieActors");
                    });

            modelBuilder.Entity<MovieImage>()
                .HasOne(mi => mi.Movie)
                .WithMany(m => m.MovieImages)
                .HasForeignKey(mi => mi.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
