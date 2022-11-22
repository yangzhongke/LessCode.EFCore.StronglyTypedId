# LessCode.EFCore.StronglyTypedId

[English version](https://github.com/yangzhongke/LessCode.EFCore.StronglyTypedId/blob/main/README.md)

基于source generator技术，这个库可以自动为Entity Framework Core中的实体类生成强类型Id类型。

强类型Id，又名“guarded keys”，是模型驱动设计（DDD）中的重要特定。使用强类型Id，开发者可以使用专用类型来保存标识值而不是用整数或者Guid等通用类型来保存。

自从.NET 7开始， Entity framework core有了对"guarded keys"的内置支持，详见官方文档[https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew?WT.mc_id=DT-MVP-5004444#improved-value-generation](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew?WT.mc_id=DT-MVP-5004444#improved-value-generation) .

根据官方文档，为了使用强类型Id，开发者必须编写非常繁多的代码，比如PersonId、PersonIdValueConverter等。这个库就是用来自动生成这些代码的。

用法:

1. Nuget包'Microsoft.EntityFrameworkCore'推荐被安装到实体类所在的项目中。

```
Install-Package Microsoft.EntityFrameworkCore
```

如果不安装'Microsoft.EntityFrameworkCore'包，强类型Id的类仍然会被生成，但是valueconverter代码不会被生成，需要开发者自己编写这些类。

2. 把Nuget包'LessCode.EFCore.StronglyTypedIdGenerator'安装到实体类所在的项目中。

```
Install-Package LessCode.EFCore.StronglyTypedIdGenerator
```

3. 把Nuget包'LessCode.EFCore.StronglyTypedIdCommons'安装到实体类所在的项目中。

```
Install-Package LessCode.EFCore.StronglyTypedIdCommons
```

4. 在需要使用强类型Id的实体类上标注[HasStronglyTypedId]。自动生成的强类型Id类型的名字是“实体类名+Id”。比如，实体类的名字是Person，自动生成的强类型Id的类型名字是PersonId。

```csharp
[HasStronglyTypedId]
internal class Person
{
	public PersonId Id { get; set; }
	public string Name { get; set; }
}
```
自动生成的PersonId的部分代码如下：

```csharp
public readonly struct PersonId
{
	public PersonId(long value) => Value = value;
	public long Value { get; }
	//and more ...
}
```

默认标识类型是long，你可以通过HasStronglyTypedId的构造函数来制定标识类型。比如，[HasStronglyTypedId(typeof(Guid))]就可以指定标识类型。

如果Nuget包'Microsoft.EntityFrameworkCore'被安装到了实体项目所在的项目中，一个名字教PersonIdValueConverter 的类型也会被生成。

5. 把Nuget包"LessCode.EFCore"安装到你的XXDbContext项目所在的项目中.

```csharp
Install-Package LessCode.EFCore
```

在OnModelCreating()方法中调用ConfigureStronglyTypedId()，在ConfigureConventions()方法中调用ConfigureStronglyTypedIdConventions()。

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
        configurationBuilder.ConfigureStronglyTypedIdConventions(this);
    }
}
```

6. 如下调用测试:

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