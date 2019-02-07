// <copyright file="ITransactionalRepository.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DomainDrivenDevelopment.Domain.Repositories
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a repository that accept transactional Start / Commit / Rollback functions.
    /// </summary>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    public interface ITransactionalRepository<TData, TKey> :
        IRepository<TData, TKey>
        where TData : class
    {
        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        /// <param name="exclusiveAccess">
        /// Indicate if we require an exclusive access to the repository.
        /// It is up to the repository to implement or not this exclusive access.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">The repository does not implement exclusive lock.</exception>
        Task BeginTransactionAsync(bool exclusiveAccess = false);

        /// <summary>
        /// Commits the current transactions.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rollback the current transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RollbackTransactionAsync();

        /// <summary>
        /// Indicates if a traansaction has been started.
        /// </summary>
        /// <returns><c>true</c> if a transacrion has been started.</returns>
        bool TransactionInProgress();
    }
}
