using System;

namespace EntitiesProject2
{
    [HasStronglyTypedId]
    public class Cat
    {
        public CatId Id { get; set; }
        public string Name { get; set; }
    }
}
