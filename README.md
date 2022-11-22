# LessCode.EFCore.StronglyTypedId

With the help of source generator, this library can generate strongly-typed-id classes automatically for entities in Entity Framework Core. 

Strongly-typed-id, aka "guarded keys", is an important feature in Domain Driven Design (DDD). With Strongly-typed-id, developers can use the instance of a specific class to hold the identity rather than an integer value or a GUID value.

Since .NET 7, Entity framework core has built-in "guarded keys" supports, see [https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#improved-value-generation](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#improved-value-generation) .

According to the documentation, developers have to write many verbose code to use Strongly-typed-id, such as PersonId, PersonIdValueConverter. This library is for generating those code automatically.

Usage:

1. The Nuget package 'Microsoft.EntityFrameworkCore' is recommended to be added to the project where entity classes reside. 

```
Install-Package Microsoft.EntityFrameworkCore
```

If the 'Microsoft.EntityFrameworkCore' is not added, Strongly-typed-id classes can be generated, but the valueconverters will not be generated.

2. Add Nuget package 'LessCode.EFCore.StronglyTypedIdGenerator' to the project where entity classes reside.

```
Install-Package LessCode.EFCore.StronglyTypedIdGenerator
```

3. Add Nuget package 'LessCode.EFCore.StronglyTypedIdCommons'  to the project where entity classes reside.

```
Install-Package LessCode.EFCore.StronglyTypedIdCommons
```

4. Add the attribute [HasStronglyTypedId] to the class that want to use the Strongly-typed-id. The name of the generated Strongly-typed-id type is 'entity name+id'. For example, if the entity name is Person, the name of the generated Strongly-typed-id type is PersonId.

```csharp
[HasStronglyTypedId]
internal class Person
{
	public PersonId Id { get; set; }
	public string Name { get; set; }
}
```
Part of the source code of the generated PersonId would be the following code:

```csharp
public readonly struct PersonId
{
	public PersonId(long value) => Value = value;
	public long Value { get; }
	//and more ...
}
```

The default type for the identity is long, you can specify the identity type with the Type parameter of HasStronglyTypedId. For example, [HasStronglyTypedId(typeof(Guid))] can specifiy the data type of the identity is Guid.

If the Nuget package 'Microsoft.EntityFrameworkCore' is added, a class named PersonIdValueConverter will be generated too.

5. Add the Nuget pacakage "LessCode.EFCore" to the project where XXDbContext resides.

```csharp
Install-Package LessCode.EFCore
```

Call ConfigureStronglyTypedId() in OnModelCreating() and call ConfigureStronglyTypedIdConventions in ConfigureConventions().

```csharp
class TestDbContext: DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Dog> Dogs { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //your code
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
```

6. Try it:

Test1:

```csharp
class Test1
{
    public static void Select1(TestDbContext ctx, DogId id)
    {
        var dog = ctx.Dogs.Find(id);
        if (dog == null)
        {
            Console.WriteLine("No dog found");
        }
        else
        {
            Console.WriteLine($"Dog: {dog.Name}");
        }
    }

    public static void Select1(TestDbContext ctx, PersonId id)
    {
        var dog = ctx.Persons.SingleOrDefault(p => p.Id == id);
        if (dog == null)
        {
            Console.WriteLine("No dog found");
        }
        else
        {
            Console.WriteLine($"Dog: {dog.Name}");
        }
    }
}
```

Program.cs

```csharp
TestDbContext ctx = new TestDbContext();

Console.WriteLine("*******************Insert*****************");
ctx.Persons.Add(new Person { Name = "tom" });
ctx.Dogs.Add(new Dog {Name="wangwang"});
ctx.SaveChanges();

Console.WriteLine("******************SelectById******************");
Test1.Select1(ctx, new DogId(Guid.Parse(xxxxxxx)));
Test1.Select1(ctx, new PersonId(3));
```