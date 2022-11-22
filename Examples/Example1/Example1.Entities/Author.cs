using Entities;

namespace Example1.Entities
{
    [HasStronglyTypedId]
    public record Author
    {
        public AuthorId Id { get; set; }
        public string Name { get; set; }
    }
}