using Example0;

using TestDbContext ctx = new TestDbContext();
Person p1 = new Person();
p1.Name = "yzk";
ctx.Persons.Add(p1);
ctx.SaveChanges();
PersonId pId1 = p1.Id;
Console.WriteLine(pId1);

Person? p2 = FindById(new PersonId(1));
Console.WriteLine(p2.Name);
ctx.Persons.Where(p=>p.Id>=new PersonId(2)).ToArray();
Person? FindById(PersonId pid)
{
    using TestDbContext ctx = new TestDbContext();
    return ctx.Persons.SingleOrDefault(p => p.Id == pid);
}