// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Readonly repository interface.
    /// </summary>
    /// <remarks>This interface represents a readonly repository contract.</remarks>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    [Obsolete("Please use Jmw.DDD.Application.Repositories.IReadOnlyRepository.")]
    public interface IReadOnlyRepository<TData, TKey> :
        Application.Repositories.IReadOnlyRepository<TData, TKey>
        where TData : class
    {
        /// <summary>
        /// Search for entities in the repository.
        /// </summary>
        /// <param name="predicate">
        /// Predicate function used to search the entities.
        /// </param>
        /// <param name="skip">How many entity of the result to skip.</param>
        /// <param name="take">How many entity of the result to return.</param>
        /// <param name="sortOrder">Sort order applied to the data.</param>
        /// <returns>A task that represents the asynchronous query operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="skip"/> or <paramref name="take"/> are lower than 0.</exception>
        Task<IEnumerable<TData>> QueryAsync(
            Expression<Func<TData, bool>> predicate,
            long skip,
            long take,
            SortOrder sortOrder = SortOrder.Ascending);

        /// <summary>
        /// Returns any entity in the repository.
        /// </summary>
        /// <param name="skip">How many entity of the result to skip.</param>
        /// <param name="take">How many entity of the result to return.</param>
        /// <param name="sortOrder">Sort order applied to the data.</param>
        /// <returns>A task that represents the asynchronous query operation.</returns>
        Task<IEnumerable<TData>> AnyAsync(long skip, long take, SortOrder sortOrder = SortOrder.Ascending);
    }
}
