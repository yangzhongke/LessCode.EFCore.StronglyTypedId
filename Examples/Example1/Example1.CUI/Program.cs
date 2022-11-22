using Entities;
using Example1.Repositories;

TestRepository rep = new TestRepository();
AuthorId aId = rep.AddNewAuthor("Zack Yang");
Console.WriteLine(aId);