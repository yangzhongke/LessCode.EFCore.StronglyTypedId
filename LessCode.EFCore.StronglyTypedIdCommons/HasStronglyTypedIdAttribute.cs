namespace System
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
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
