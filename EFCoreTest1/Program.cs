using EFCoreTest1;
using System.Linq.Expressions;

TestDbContext ctx = new TestDbContext();
ctx.Persons.Add(new Person { Name = "tom" });
ctx.Dogs.Add(new Dog {Name="wangwang"});
ctx.SaveChanges();