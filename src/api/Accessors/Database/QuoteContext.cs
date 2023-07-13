using Api.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Api.Accessors.Database
{
    public class QuoteContext : DbContext
    {
        public DbSet<Quote> Quotes { get; set; }
        private readonly DatabaseConfiguration _configuration;

        public QuoteContext(DatabaseConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(_configuration.ConnectionString);
    }
}