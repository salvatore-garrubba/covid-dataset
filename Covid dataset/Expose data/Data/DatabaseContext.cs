using Expose_data.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Expose_data.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Filedoc> Files { get; set; }
    }
}
