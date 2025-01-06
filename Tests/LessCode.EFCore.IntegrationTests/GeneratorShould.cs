using EFCoreTest1;
using FluentAssertions;

namespace LessCode.EFCore.IntegrationTests
{
    public class GeneratorShould
    {
        [Fact]
        public void WorkWithDefaultLongId()
        {
            TestDbContext dbContext = new();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            Person personToBeInserted = new() { Name = "John" };
            dbContext.Persons.Add(personToBeInserted);
            dbContext.SaveChanges();
            Person personRetrieved = dbContext.Persons.Single();
            personRetrieved.Name.Should().Be("John");
            PersonId idRetrieved = personRetrieved.Id;
            idRetrieved.Value.Should().NotBe(default);
            idRetrieved.Should().Be(personToBeInserted.Id);

            dbContext.Persons.SingleOrDefault(e=>e.Id == idRetrieved).Should().Be(personRetrieved);
        }

        [Fact]
        public void WorkWithDefaultGuid()
        {
            TestDbContext dbContext = new();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            Person2 personToBeInserted = new() { Name = "John" };
            dbContext.Person2s.Add(personToBeInserted);
            dbContext.SaveChanges();
            Person2 personRetrieved = dbContext.Person2s.Single();
            personRetrieved.Name.Should().Be("John");
            Person2Id idRetrieved = personRetrieved.Id;
            idRetrieved.Value.Should().NotBe(Guid.Empty);
            idRetrieved.Should().Be(personToBeInserted.Id);
        }

        [Fact]
        public void WorkWithExplicitLongId()
        {
            TestDbContext dbContext = new();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            PersonId personId = new(1);
            Person personToBeInserted = new() { Name = "John", Id= personId };
            dbContext.Persons.Add(personToBeInserted);
            dbContext.SaveChanges();
            Person personRetrieved = dbContext.Persons.Single();
            personRetrieved.Name.Should().Be("John");
            PersonId idRetrieved = personRetrieved.Id;
            idRetrieved.Value.Should().NotBe(default);
            idRetrieved.Should().Be(personId);
        }

        [Fact]
        public void WorkWithExplicitGuid()
        {
            TestDbContext dbContext = new();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            Person2Id personId = new();
            Person2 personToBeInserted = new() { Name = "John",Id=personId };
            dbContext.Person2s.Add(personToBeInserted);
            dbContext.SaveChanges();
            Person2 personRetrieved = dbContext.Person2s.Single();
            personRetrieved.Name.Should().Be("John");
            personRetrieved.Id.Should().Be(personId);
        }
    }
}