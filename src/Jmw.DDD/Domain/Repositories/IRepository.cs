// <copyright file="IRepository.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Domain.Repositories
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface de base d'un repository.
    /// </summary>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    public interface IRepository<TData, TKey> :
        IReadOnlyRepository<TData, TKey>
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
        /// <param name="entity">Entity to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <c>null</c>.</exception>
        Task UpdateAsync(TData entity);

        /// <summary>
        /// Deletes an existing element to the repository.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <c>null</c>.</exception>
        Task DeleteAsync(TData entity);
    }
}
