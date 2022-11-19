using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace LessCode.EFCore
{
    public class StronglyTypedIdGuidValueGenerator<TEntityId> : ValueGenerator<TEntityId>
        where TEntityId: struct
    {
        public override bool GeneratesTemporaryValues => false;

        public override TEntityId Next(EntityEntry entry)
        {
            var constructor = typeof(TEntityId).GetConstructor(new Type[] { typeof(Guid) });
            TEntityId newObj = (TEntityId)constructor.Invoke(new object[] { Guid.NewGuid() });
            return newObj;
        }
    }
}
