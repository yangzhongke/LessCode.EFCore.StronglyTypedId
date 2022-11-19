using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LessCode.EFCore
{
    public static class StronglyTypedIdExtensions
    {
        public static void ConfigureAllStronglyTypedIds(this ModelConfigurationBuilder builder,params Assembly[] assemblies)
        {
            bool stronglyTypedIdFound = false;
            foreach(var type in assemblies.SelectMany(a=>a.GetTypes()))
            {
                var valueConverterAttr = type.GetCustomAttributes<StronglyTypedIdValueConverterAttribute>().SingleOrDefault();
                if (valueConverterAttr == null) continue;
                Type typeId = valueConverterAttr.IdType;
                builder.Properties(typeId, s => {
                    s.HaveConversion(type);
                });
                stronglyTypedIdFound = true;
            }
            if(!stronglyTypedIdFound)
            {
                throw new InvalidOperationException($"No automatically generated ValueConverter types were not found. \r\nSolution 1: Add reference of Microsoft.EntityFrameworkCore to the project of entity types;\r\n Solution 2: Write the ValueConverter types manually and remove the calling to {nameof(ConfigureAllStronglyTypedIds)}.");
            }
        }

        public static void ConfigureAllStronglyTypedIds(this ModelConfigurationBuilder builder)
        {
            ConfigureAllStronglyTypedIds(builder, Assembly.GetCallingAssembly());
        }
    }
}