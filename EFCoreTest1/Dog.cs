namespace EFCoreTest1;

[HasStronglyTypedId(typeof(Guid))]
public class Dog
{
public DogId Id { get; set; }
public string Name { get; set; }
}
