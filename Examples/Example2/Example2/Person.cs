namespace Example2
{
    [HasStronglyTypedId(typeof(Guid))]
    class Person
    {
        public PersonId Id { get; set; }
        public string Name { get; set; }
    }
}
