using Entities;
using Example1.Entities;
using Microsoft.EntityFrameworkCore;

namespace Example1.Repositories
{
    public class TestRepository
    {
        public AuthorId AddAuthor(string name)
        {
            using Example1DbContext ctx = new Example1DbContext();
            Author a = new Author();
            a.Name = name;
            ctx.Authors.Add(a);
            ctx.SaveChanges();
            return a.Id;
        }

        public Author? FindById(AuthorId authorId)
        {
            using Example1DbContext ctx = new Example1DbContext();
            return ctx.Authors.SingleOrDefault(a=>a.Id==authorId);
        }

        public Author[] FindAuthorsByName(string name)
        {
            using Example1DbContext ctx = new Example1DbContext();
            return ctx.Authors.Where(a=>a.Name==name).ToArray();
        }

        public BookId AddBook(string name,double price,AuthorId authorId)
        {
            using Example1DbContext ctx = new Example1DbContext();
            Author author = ctx.Authors.Single(a=>a.Id==authorId);
            Book b = new Book { Author= author ,Name=name,Price=price};
            ctx.Books.Add(b);
            ctx.SaveChanges();
            return b.Id;
        }

        public Book? FindById(BookId bookId)
        {
            using Example1DbContext ctx = new Example1DbContext();
            return ctx.Books.Include(b=>b.Author).SingleOrDefault(b=>b.Id==bookId);
        }

        public Book[] FindBooksByName(string name)
        {
            using Example1DbContext ctx = new Example1DbContext();
            return ctx.Books.Include(b => b.Author).Where(b => b.Name == name).ToArray();
        }
    }
}
