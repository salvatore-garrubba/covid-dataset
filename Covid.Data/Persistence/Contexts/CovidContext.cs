using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using Covid.Data.Domain.Models;
namespace Covid.Data.Persistence.Contexts
{
    public class CovidContext: DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory
        = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name
                        && level == LogLevel.Information)
                    .AddConsole();
            });
        public DbSet<Category> Categories { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public CovidContext(DbContextOptions<CovidContext> options) : base(options)
        {                   
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Category>().ToTable("categories");
            builder.Entity<Category>().HasKey(c => c.Id);
            builder.Entity<Category>().Property(c => c.Id).IsRequired().ValueGeneratedNever();//.ValueGeneratedOnAdd();
            //builder.Entity<Category>().Property(p => p.Name).IsRequired().HasMaxLength(30);
            builder.Entity<Category>().HasMany(c => c.Questions).WithOne(q => q.Category).HasForeignKey(q => q.CategoryId);

            builder.Entity<Question>().ToTable("questions");
            builder.Entity<Question>().HasKey(q => q.Id);
            builder.Entity<Question>().Property(q => q.Id).IsRequired().ValueGeneratedNever();
            builder.Entity<Question>().Property(q => q.Text).IsRequired().HasMaxLength(4000);
            builder.Entity<Question>().HasMany(q => q.Answers).WithOne(a => a.Question).HasForeignKey(a => a.QuestionId);

            
            builder.Entity<Answer>().ToTable("answers");
            builder.Entity<Answer>().HasKey(a => a.Id);
            //builder.Entity<Answer>().Property(a => a.Id).UseIdentityColumn();//.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);//.IsRequired().ValueGeneratedNever();
            builder.Entity<Answer>().Property(a => a.Id).IsUnicode(false).HasMaxLength(255).IsRequired().ValueGeneratedNever();
            builder.Entity<Answer>().Property(a => a.Timestamp).IsRequired();
            builder.Entity<Answer>().Property(a => a.Text).HasMaxLength(4000);        
        }

       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
        }
         
    }
}