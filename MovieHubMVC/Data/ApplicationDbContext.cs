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

        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Cinema> Cinemas { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<MovieImage> MovieImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>()
                .HasOne(d => d.Category)
                .WithMany(p => p.Movies)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Movie>()
                .HasOne(d => d.Cinema)
                .WithMany(p => p.Movies)
                .HasForeignKey(d => d.CinemaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Movie>()
                .HasMany(d => d.Actors)
                .WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieActor",
                    r => r.HasOne<Actor>().WithMany().HasForeignKey("ActorId"),
                    l => l.HasOne<Movie>().WithMany().HasForeignKey("MovieId"),
                    j =>
                    {
                        j.HasKey("MovieId", "ActorId");
                        j.ToTable("MovieActors");
                    }
                );

            modelBuilder.Entity<MovieImage>()
                .HasOne(d => d.Movie)
                .WithMany(p => p.MovieImages)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
