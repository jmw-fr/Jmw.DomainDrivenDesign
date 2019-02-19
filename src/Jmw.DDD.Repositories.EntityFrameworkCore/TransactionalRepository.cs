// <copyright file="TransactionalRepository.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCore
{
    using System;
    using System.Threading.Tasks;
    using Jmw.DDD.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Basic implementation of <see cref="ITransactionalRepository{TData, TKey}"/>.
    /// </summary>
    /// <typeparam name="TContext">Entity DbContext class.</typeparam>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    public abstract class TransactionalRepository<TContext, TData, TKey> :
        Repository<TContext, TData, TKey>,
        ITransactionalRepository<TData, TKey>
        where TContext : DbContext
        where TData : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalRepository{TContext, TData, TKey}"/> class.
        /// </summary>
        /// <param name="context">EntityFramework context to use.</param>
        /// <param name="propertySelector">Selector of the DbSet property of <paramref name="context"/>.</param>
        public TransactionalRepository(
            TContext context,
            Func<TContext, DbSet<TData>> propertySelector)
            : base(context, propertySelector)
        {
        }

        /// <inheritdoc/>
        public async Task BeginTransactionAsync(bool exclusiveAccess = false)
        {
            if (Configuration.Context.Database.CurrentTransaction is null)
            {
                await Configuration.Context.Database.BeginTransactionAsync();
            }

            if (exclusiveAccess)
            {
                if (Configuration.Context.Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
                {
                    string sql = $"LOCK TABLE \"{Configuration.Schema}\".\"{Configuration.TableName}\" IN ACCESS EXCLUSIVE MODE;";

                    await Configuration.Context.Database.ExecuteSqlCommandAsync(sql);
                }
                else
                {
                    throw new InvalidOperationException("Exclusive lock is not implemented with this driver.");
                }
            }
        }

        /// <inheritdoc/>
        public async Task CommitTransactionAsync()
        {
            if (Configuration.Context.Database.CurrentTransaction != null)
            {
                Configuration.Context.Database.CommitTransaction();
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task RollbackTransactionAsync()
        {
            if (Configuration.Context.Database.CurrentTransaction != null)
            {
                Configuration.Context.Database.RollbackTransaction();
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public bool TransactionInProgress()
        {
            return Configuration.Context.Database.CurrentTransaction != null;
        }
    }
}
