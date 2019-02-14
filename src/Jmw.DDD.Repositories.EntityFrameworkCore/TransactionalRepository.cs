// <copyright file="TransactionalRepository.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Jmw.DDD.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Basic implementation of <see cref="ITransactionalRepository{TData, TKey}"/>.
    /// </summary>
    /// <typeparam name="TContext">Entity DbContext class.</typeparam>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    /// <typeparam name="TOrderBy">Order by property type.</typeparam>
    public class TransactionalRepository<TContext, TData, TKey, TOrderBy> :
        Repository<TContext, TData, TKey, TOrderBy>,
        ITransactionalRepository<TData, TKey>
        where TContext : DbContext
        where TData : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalRepository{TContext, TData, TKey, TOrderBy}"/> class.
        /// </summary>
        /// <param name="context">EntityFramework context to use.</param>
        /// <param name="propertySelector">Selector of the DbSet property of <paramref name="context"/>.</param>
        /// <param name="orderBySelector">Selector of <paramref name="propertySelector"/> property used to order by.</param>
        /// <param name="includes">Referenced properties to include during selection of data.</param>
        public TransactionalRepository(
            TContext context,
            Func<TContext, DbSet<TData>> propertySelector,
            Expression<Func<TData, TOrderBy>> orderBySelector = null,
            IEnumerable<string> includes = null)
            : base(context, propertySelector, orderBySelector, includes)
        {
        }

        /// <inheritdoc/>
        public async Task BeginTransactionAsync(bool exclusiveAccess = false)
        {
            if (Context.Database.CurrentTransaction is null)
            {
                await Context.Database.BeginTransactionAsync();
            }

            if (exclusiveAccess)
            {
                if (this.Context.Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
                {
                    string sql = $"LOCK TABLE \"{Schema}\".\"{TableName}\" IN ACCESS EXCLUSIVE MODE;";

                    await Context.Database.ExecuteSqlCommandAsync(sql);
                }

                throw new InvalidOperationException("exclusive lock is not implemented with this driver.");
            }
        }

        /// <inheritdoc/>
        public async Task CommitTransactionAsync()
        {
            if (Context.Database.CurrentTransaction != null)
            {
                Context.Database.CommitTransaction();
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task RollbackTransactionAsync()
        {
            if (Context.Database.CurrentTransaction != null)
            {
                Context.Database.RollbackTransaction();
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public bool TransactionInProgress()
        {
            return Context.Database.CurrentTransaction != null;
        }
    }
}
