// See https://aka.ms/new-console-template for more information
using EFCoreTest1;
using System.Linq.Expressions;

TestDbContext ctx = new TestDbContext();
ctx.Persons.ToArray();