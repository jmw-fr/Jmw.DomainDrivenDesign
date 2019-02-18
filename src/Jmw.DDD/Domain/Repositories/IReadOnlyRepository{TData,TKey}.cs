// <copyright file="IReadOnlyRepository{TData,TKey}.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

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
    public interface IReadOnlyRepository<TData, TKey>
        where TData : class
    {
        /// <summary>
        /// Returns the entities count in the repository.
        /// </summary>
        /// <param name="predicate">
        /// Predicate function used to search the entities.
        /// If <c>null</c> then the first entity of all the repository is searched.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous count operation. The task result contains the entities count.
        /// </returns>
        Task<long> CountAsync(Expression<Func<TData, bool>> predicate = null);

        /// <summary>
        /// Asynchronously finds an entity with the given Id.
        /// </summary>
        /// <param name="key">Key of the entity.</param>
        /// <returns>
        /// A task that represents the asynchronous find operation. The task result contains the entity found, or null.
        /// </returns>
        Task<TData> FindAsync(TKey key);

        /// <summary>
        /// Returns the first entity found in the repository.
        /// </summary>
        /// <param name="predicate">
        /// Predicate function used to search the entities.
        /// If <c>null</c> then the first entity of all the repository is searched.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous find operation. The task result contains the entity found, or null.
        /// </returns>
        Task<TData> FirstAsync(Expression<Func<TData, bool>> predicate);

        /// <summary>
        /// Returns the last entity found in the repository.
        /// </summary>
        /// <param name="predicate">
        /// Predicate function used to search the entities.
        /// If <c>null</c> then the last entity of all the repository is searched.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous find operation. The task result contains the entity found, or null.
        /// </returns>
        Task<TData> LastAsync(Expression<Func<TData, bool>> predicate);

        /// <summary>
        /// Search for entities in the repository.
        /// </summary>
        /// <param name="predicate">
        /// Predicate function used to search the entities.
        /// </param>
        /// <param name="skip">How many entity of the result to skip.</param>
        /// <param name="take">How many entity of the result to return.</param>
        /// <returns>A task that represents the asynchronous query operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="skip"/> or <paramref name="take"/> are lower than 0.</exception>
        Task<IEnumerable<TData>> QueryAsync(Expression<Func<TData, bool>> predicate, long skip, long take);

        /// <summary>
        /// Returns any entity in the repository.
        /// </summary>
        /// <param name="skip">How many entity of the result to skip.</param>
        /// <param name="take">How many entity of the result to return.</param>
        /// <returns>A task that represents the asynchronous query operation.</returns>
        Task<IEnumerable<TData>> AnyAsync(long skip, long take);
    }
}
