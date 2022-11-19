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
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureStronglyTypedId();
            //var props = modelBuilder.Entity(typeof(Person)).Metadata.GetProperties();
            //modelBuilder.Entity(typeof(Person)).Property("Id").ValueGeneratedOnAdd();
            /*
            modelBuilder.Entity<Person>().Property("Id")
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Dog>().Property(e => e.Id)
                .ValueGeneratedOnAdd();*/
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.ConfigureStronglyTypedIdConventions();
        }
    }
}
