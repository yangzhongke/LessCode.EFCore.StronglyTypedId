using LessCode.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Example2
{
    class TestDbContext:DbContext
    {
        public DbSet<Person> Persons { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connStr = "Server=.;Database=example2;Integrated Security=true;TrustServerCertificate=true;";
            optionsBuilder.UseSqlServer(connStr);
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
