using Entities;
using Example1.Entities;
using Example1.Repositories;

TestRepository rep = new TestRepository();
AuthorId aId = rep.AddAuthor("Zack Yang");
Console.WriteLine($"Author Created:{aId}");

BookId bId = rep.AddBook("ASP.NET Core Jishuneimu", 119, aId);
Console.WriteLine($"Book Created:{bId}");

Book? book1 = rep.FindById(bId);
Console.WriteLine(book1);

var books = rep.FindBooksByName("ASP.NET Core Jishuneimu");
foreach(var b in books)
{
    Console.WriteLine(b);
}
