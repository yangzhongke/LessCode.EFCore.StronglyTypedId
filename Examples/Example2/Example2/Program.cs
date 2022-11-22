using Example2;

TestDbContext ctx = new TestDbContext();
Person p1 = new Person();
p1.Name = "yzk";
ctx.Persons.Add(p1);
ctx.SaveChanges();
Console.WriteLine(p1.Id);

Person p2 = new Person();
Guid guid1 = Guid.Parse("f8fc2b0d-7c1d-4899-80ab-bf0d95e7ea41");
p2.Id = new PersonId(guid1);
p2.Name = "Mike";
ctx.Persons.Add(p2);
ctx.SaveChanges();

