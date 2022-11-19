using EFCoreTest1;

TestDbContext ctx = new TestDbContext();

Console.WriteLine("*******************Insert*****************");
ctx.Persons.Add(new Person { Name = "tom" });
ctx.Dogs.Add(new Dog {Name="wangwang"});
ctx.SaveChanges();

Console.WriteLine("******************Select All******************");
foreach (var d in ctx.Dogs)
{
    Console.WriteLine(d.Id);
}

Console.WriteLine("******************SelectById******************");
Test1.Select1(ctx, new DogId(Guid.Parse("EBC47B84-8799-4990-A67F-321921255D10")));
Test1.Select1(ctx, new PersonId(3));