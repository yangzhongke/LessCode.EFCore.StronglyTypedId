﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.Reflection;

namespace LessCode.EFCore
{
    public static class StronglyTypedIdExtensions
    {
        public static void ConfigureStronglyTypedId(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var entityClrType = entityType.ClrType;
                bool hasStronglyTypedIdAttr = entityClrType.GetCustomAttributes<HasStronglyTypedIdAttribute>().Any();
                if (!hasStronglyTypedIdAttr) continue;
                var keys = entityType.GetKeys();
                foreach (var key in keys)
                {
                    if(key.IsPrimaryKey()&& key.Properties.Count == 1)
                    {
                        var keyProp = key.Properties.Single();
                        ConfigureStronglyIdProp(keyProp);
                    }
                }
            }
        }

        private static void ConfigureStronglyIdProp(IMutableProperty keyProp)
        {
            Type keyClrType = keyProp.ClrType;//PersonId
            Type idClrType = keyClrType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).PropertyType;//long
            if (idClrType == typeof(int) || idClrType == typeof(long))
            {
                keyProp.ValueGenerated = ValueGenerated.OnAdd;
            }
            else if (idClrType == typeof(Guid))
            {
                keyProp.ValueGenerated = ValueGenerated.OnAdd;
                keyProp.SetValueGeneratorFactory((p, et) =>
                {
                    Type typeValueGenerator =
                        typeof(StronglyTypedIdGuidValueGenerator<>).MakeGenericType(keyClrType);
                    var valueGenerator = Activator.CreateInstance(typeValueGenerator);
                    return (ValueGenerator)valueGenerator;
                });
            }
        }

        private static IEnumerable<Assembly> GetAssembliesOfEntityTypes(DbContext dbCtx)
        {
            foreach (var p in dbCtx.GetType().GetProperties())
            {
                var pt = p.PropertyType;
                if(pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(DbSet<>))
                {
                    Type entityType = pt.GenericTypeArguments[0];
                    yield return entityType.Assembly;
                }
            }
        }

        public static void ConfigureStronglyTypedIdConventions(this ModelConfigurationBuilder builder, DbContext dbCtx)
        {
            var types = GetAssembliesOfEntityTypes(dbCtx).SelectMany(a => a.GetTypes());
            bool stronglyTypedIdFound = false;
            foreach (var type in types)
            {
                var valueConverterAttr = type.GetCustomAttributes<StronglyTypedIdValueConverterAttribute>().SingleOrDefault();
                if (valueConverterAttr == null) continue;
                Type typeId = valueConverterAttr.IdType;
                builder.Properties(typeId, s =>
                {
                    s.HaveConversion(type);
                });
                stronglyTypedIdFound = true;
            }
            if (!stronglyTypedIdFound)
            {
                throw new InvalidOperationException($"No automatically generated ValueConverter types were not found. \r\nSolution 1: Add reference of Microsoft.EntityFrameworkCore to the project of entity types;\r\n Solution 2: Write the ValueConverter types manually and remove the calling to {nameof(ConfigureStronglyTypedIdConventions)}.");
            }
        }
    }
}