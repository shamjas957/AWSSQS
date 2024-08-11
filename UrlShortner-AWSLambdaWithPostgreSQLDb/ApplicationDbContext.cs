using Microsoft.EntityFrameworkCore;
using UrlShortner_AWSLambdaWithPostgreSQLDb.Entities;
using UrlShortner_AWSLambdaWithPostgreSQLDb.Services;

namespace UrlShortner_AWSLambdaWithPostgreSQLDb
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options) { }
        
        public DbSet<ShortenedUrl> ShortenedUrl { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortenedUrl>(builder =>
            {
                builder.Property(s => s.Code).HasMaxLength(UrlShorteningService.NumberOfCharactersInShortLink);
                builder.HasIndex(s => s.Code).IsUnique();
            }); 
        }
    }
}
