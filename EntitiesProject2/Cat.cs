using System;

namespace EntitiesProject2
{
    [HasStronglyTypedId]
    public record Cat
    {
        public CatId Id { get; set; }
        public string Name { get; set; }
    }
}
