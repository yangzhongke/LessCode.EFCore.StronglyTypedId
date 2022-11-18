namespace EFCoreTest1
{
    //[HasStronglyTypedId(typeof(Guid))]
    [HasStronglyTypedId]
    //[HasStronglyTypedId("System.Int64")]
    internal class Person
    {
        public PersonId Id { get; set; }
        public string Name { get; set; }
    }
}
