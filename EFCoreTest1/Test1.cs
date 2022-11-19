namespace EFCoreTest1
{
    class Test1
    {
        public static void Select1(TestDbContext ctx, DogId id)
        {
            var dog = ctx.Dogs.Find(id);
            if (dog == null)
            {
                Console.WriteLine("No dog found");
            }
            else
            {
                Console.WriteLine($"Dog: {dog.Name}");
            }
        }

        public static void Select1(TestDbContext ctx, PersonId id)
        {
            var dog = ctx.Persons.SingleOrDefault(p => p.Id == id);
            if (dog == null)
            {
                Console.WriteLine("No dog found");
            }
            else
            {
                Console.WriteLine($"Dog: {dog.Name}");
            }
        }
    }
}
