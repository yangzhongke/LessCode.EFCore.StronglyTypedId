namespace EFCoreTest1
{
    [HasStronglyTypedId(typeof(Guid))]
    internal class Person2
    {
        public Person2Id Id { get; set; }
        public string Name { get; set; }
    }
}
