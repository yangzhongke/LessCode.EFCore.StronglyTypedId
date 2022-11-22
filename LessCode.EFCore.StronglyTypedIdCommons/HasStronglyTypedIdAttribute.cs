namespace System
{
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Struct| AttributeTargets.Interface, Inherited =false)]
    public class HasStronglyTypedIdAttribute:Attribute
    {
        public HasStronglyTypedIdAttribute()
        {
        }

        public HasStronglyTypedIdAttribute(Type idType)
        {
        }
        public HasStronglyTypedIdAttribute(string typeName)
        {
        }
    }
}
