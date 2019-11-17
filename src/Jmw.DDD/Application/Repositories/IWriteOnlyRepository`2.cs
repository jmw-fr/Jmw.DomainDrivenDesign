// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Application.Repositories
{
    using System;

    using System.Threading.Tasks;

    /// <summary>
    /// Readonly repository interface.
    /// </summary>
    /// <remarks>This interface represents a readonly repository contract.</remarks>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    public interface IWriteOnlyRepository<TData, TKey>
        where TData : class
    {
        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">Entity to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <c>null</c>.</exception>
        Task InsertAsync(TData entity);

        /// <summary>
        /// Updates an existing element to the repository.
        /// </summary>
        /// <param name="entity">Etity to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <c>null</c>.</exception>
        Task UpdateAsync(TData entity);
    }
}
