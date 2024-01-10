using Microsoft.EntityFrameworkCore;
using Store_files.Data.Models;

namespace Store_files.Data
{
    public class DatabaseContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=covid_dataset;User Id=postgres;Password=root");

        public DbSet<Filedoc> Files { get; set; }
    }
}
