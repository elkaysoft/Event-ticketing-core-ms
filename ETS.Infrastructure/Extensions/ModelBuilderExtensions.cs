using ETS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ETS.Infrastructure.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void RegisterAllEntities(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            if(modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            if(assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            IEnumerable<Type> types = assemblies.AsParallel()
                .SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && c.BaseType != null &&
                 c.BaseType.IsGenericType && c.BaseType.GetGenericTypeDefinition() == typeof(Entity<>));

            types.ToList().ForEach(type => modelBuilder.Entity(type));
        }
    }
}
