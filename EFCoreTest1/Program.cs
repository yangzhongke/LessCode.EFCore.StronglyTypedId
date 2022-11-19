using EFCoreTest1;

TestDbContext ctx = new TestDbContext();
ctx.Persons.Add(new Person { Name = "tom" });
ctx.Dogs.Add(new Dog {Name="wangwang"});
ctx.SaveChanges();
foreach(var d in ctx.Dogs)
{
    Console.WriteLine(d.Id);
}