using LessCode.EFCore;
using Microsoft.EntityFrameworkCore;

namespace EFCoreTest1
{
    internal class TestDbContext: DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Dog> Dogs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connStr = "Server=.;Database=demo1;Integrated Security=true;TrustServerCertificate=true;";
            optionsBuilder.UseSqlServer(connStr);
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
            configurationBuilder.ConfigureStronglyTypedIdConventions();
        }
    }
}
