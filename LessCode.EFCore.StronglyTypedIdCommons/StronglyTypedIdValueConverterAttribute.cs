namespace System
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class StronglyTypedIdValueConverterAttribute: Attribute
    {
        public Type IdType { get; set; }
        public StronglyTypedIdValueConverterAttribute(Type idType)
        {
            this.IdType = idType;
        }
    }
}
