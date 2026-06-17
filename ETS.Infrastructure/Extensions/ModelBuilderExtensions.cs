using ETS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace ETS.Infrastructure.Extensions;

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

    public static void ApplySoftDeleteFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(EntityBase).IsAssignableFrom(e.ClrType)))
        {
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.PropertyOrField(parameter, nameof(EntityBase.IsDeleted));
            var comparison = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(comparison, parameter);
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
}
 
