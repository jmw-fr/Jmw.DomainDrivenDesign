// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Repositories.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Jmw.ComponentModel.DataAnnotations;
    using Jmw.DDD.Application;
    using Jmw.DDD.Application.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Basic implementation of <see cref="ITransactionalRepository{TData, TKey}"/>.
    /// </summary>
    /// <typeparam name="TContext">Entity DbContext class.</typeparam>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    public abstract class TransactionalRepository<TContext, TData, TKey> :
        RepositoryBase<TContext, TData>,
        ITransactionalRepository<TData, TKey>,
#pragma warning disable CS0618 // Type or member is obsolete
        Domain.Repositories.ITransactionalRepository<TData, TKey>
#pragma warning restore CS0618 // Type or member is obsolete
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
        public async Task<TData> FindAsync(TKey key)
        {
            Logger.Debug("TransactionalRepository::FindAsync");

            TData entity = await Configuration.DbSet.FindAsync(key);

            foreach (var include in Configuration.IncludeProperties)
            {
                var reference = Configuration.Context.Entry(entity).References.Where(r => r.Metadata.Name == include).FirstOrDefault();

                if (reference != null)
                {
                    await reference.LoadAsync();
                }

                var collection = Configuration.Context.Entry(entity).Collections.Where(r => r.Metadata.Name == include).FirstOrDefault();

                if (collection != null)
                {
                    await collection.LoadAsync();
                }
            }

            return entity;
        }

        /// <summary>
        /// Implementation of <see cref="IReadOnlyRepository{TData, TKey}.CountAsync"/>.
        /// </summary>
        /// <param name="predicate">
        /// Predicate function used to search the entities.
        /// If <c>null</c> then the first entity of all the repository is searched.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous count operation. The task result contains the entities count.
        /// </returns>
        public async Task<long> CountAsync(Expression<Func<TData, bool>> predicate = null)
        {
            Logger.Debug("TransactionalRepository::CountAsync");

            if (predicate is null)
            {
                return await Configuration.DbSet.LongCountAsync();
            }
            else
            {
                return await Configuration.DbSet.LongCountAsync(predicate);
            }
        }

        /// <summary>
        /// Implementation of <see cref="IReadOnlyRepository{TData, TKey}.FirstAsync"/>.
        /// </summary>
        /// <param name="predicate">Optional predicate function.</param>
        /// <returns>Found data or <c>null</c>.</returns>
        public async Task<TData> FirstAsync(Expression<Func<TData, bool>> predicate = null)
        {
            Logger.Debug("TransactionalRepository::FirstAsync");

            return await PrepareQuery(predicate, 0, 1, SortOrder.Ascending).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Implementation of <see cref="IReadOnlyRepository{TData, TKey}.LastAsync"/>.
        /// </summary>
        /// <param name="predicate">Optional predicate function.</param>
        /// <returns>Found data or <c>null</c>.</returns>
        public async Task<TData> LastAsync(Expression<Func<TData, bool>> predicate = null)
        {
            Logger.Debug("TransactionalRepository::LastAsync");

            return await PrepareQuery(predicate, 0, 1, SortOrder.Descending).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Implementation of <see cref="IReadOnlyRepository{TData, TKey}.AnyAsync"/>.
        /// </summary>
        /// <param name="skip">Optional predicate function.</param>
        /// <param name="take">How many entity of the result to return.</param>
        /// <param name="sortOrder">Sort order applied to the data.</param>
        /// <returns>Found data or <c>null</c>.</returns>
        public async Task<IEnumerable<TData>> AnyAsync(
            long skip,
            long take,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            Logger.Debug("TransactionalRepository::AnyAsync");

            return await PrepareQuery(null, skip, take, sortOrder).ToListAsync();
        }

        /// <summary>
        /// Implementation of <see cref="IReadOnlyRepository{TData, TKey}.QueryAsync"/>.
        /// </summary>
        /// <param name="predicate">
        /// Predicate function used to search the entities.
        /// </param>
        /// <param name="skip">How many entity of the result to skip.</param>
        /// <param name="take">How many entity of the result to return.</param>
        /// <param name="sortOrder">Sort order applied to the data.</param>
        /// <returns>Found data or <c>null</c>.</returns>
        public async Task<IEnumerable<TData>> QueryAsync(
            Expression<Func<TData, bool>> predicate,
            long skip,
            long take,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            Logger.Debug("TransactionalRepository::QueryAsync");

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), "Use AnyAsync instead.");
            }

            return await PrepareQuery(predicate, skip, take, sortOrder).ToListAsync();
        }

        /// <inheritdoc />
        public async Task InsertAsync(TData entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.ValidateModel();

            Configuration.DbSet.Add(entity);

            await Configuration.Context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(TData entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.ValidateModel();

            Configuration.DbSet.Update(entity);

            await Configuration.Context.SaveChangesAsync();
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
