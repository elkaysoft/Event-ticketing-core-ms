using Microsoft.EntityFrameworkCore;

namespace ETS.Domain.Contracts
{
    public interface IDbContext
    {
        /// <summary>
        /// Gets the underlying <see cref="DbContext"/> instance.
        /// </summary>
        /// <returns></returns>
        DbContext GetDbContext();

        /// <summary>
        /// Saves all changes made in this context to the database
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
