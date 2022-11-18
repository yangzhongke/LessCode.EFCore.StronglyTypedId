namespace EFCoreTest1
{
    //[HasStronglyTypedId(typeof(long))]
    [HasStronglyTypedId]
    internal class Person
    {
        public PersonId Id { get; set; }
        public string Name { get; set; }
    }
}
