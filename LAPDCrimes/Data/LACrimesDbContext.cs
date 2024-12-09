using LAPDCrimes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LAPDCrimes.Data
{
    public class LACrimesDbContext : IdentityDbContext
    {
        private readonly IConfiguration _configuration;

        public LACrimesDbContext( DbContextOptions options, IConfiguration configuration): base(options) {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("LACrimes"));
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CrimeUser>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
            });
            // Additional configuration can be added here
        }
        public DbSet<CrimeUser> CrimeUsers { get; set; }

    }
}
