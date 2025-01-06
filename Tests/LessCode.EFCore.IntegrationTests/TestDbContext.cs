using LessCode.EFCore;
using Microsoft.EntityFrameworkCore;

namespace EFCoreTest1
{
    internal class TestDbContext: DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Person2> Person2s { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Database.db");
            optionsBuilder.LogTo(Console.WriteLine);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureStronglyTypedId();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.ConfigureStronglyTypedIdConventions(this);
        }
    }
}
