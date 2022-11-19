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
            string connStr = "Server=.;Database=demo1;User Id=sa;Password=dLLikhQWy5TBz1uM";
            optionsBuilder.UseSqlServer(connStr);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.ConfigureAllStronglyTypedIds();
        }
    }
}
