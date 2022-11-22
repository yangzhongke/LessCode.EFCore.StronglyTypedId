using Entities;
using Example1.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example1.Repositories
{
    public class TestRepository
    {
        public AuthorId AddNewAuthor(string name)
        {
            using Example1DbContext ctx = new Example1DbContext();
            Author a = new Author();
            a.Name = name;
            ctx.Authors.Add(a);
            ctx.SaveChanges();
            return a.Id;
        }
    }
}
