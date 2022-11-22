using Entities;

namespace Example1.Entities
{
    [HasStronglyTypedId]
    public class Author
    {
        public AuthorId Id { get; set; }
        public string Name { get; set; }
    }
}