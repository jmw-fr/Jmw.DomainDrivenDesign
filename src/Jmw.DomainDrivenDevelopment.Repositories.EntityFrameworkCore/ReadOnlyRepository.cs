// <copyright file="ReadOnlyRepository.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DomainDrivenDevelopment.Repositories.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Jmw.DomainDrivenDevelopment.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Storage;
    using NLog;

    /// <summary>
    /// Basic implementation of <see cref="IReadOnlyRepository{TData, TKey}"/> for EntityFrameworkCore.
    /// </summary>
    /// <typeparam name="TContext">Entity DbContext class.</typeparam>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    /// <typeparam name="TOrderBy">Order by property type.</typeparam>
    public class ReadOnlyRepository<TContext, TData, TKey, TOrderBy> :
        IReadOnlyRepository<TData, TKey>
        where TContext : DbContext
        where TData : class
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepository{TContext, TData, TKey, TOrderBy}"/> class.
        /// </summary>
        /// <param name="context">EntityFramework context to use.</param>
        /// <param name="propertySelector">Selector of the DbSet property of <paramref name="context"/>.</param>
        /// <param name="orderBySelector">Selector of <paramref name="propertySelector"/> property used to order by.</param>
        /// <param name="includes">Referenced properties to include during selection of data.</param>
        public ReadOnlyRepository(
            TContext context,
            Func<TContext, DbSet<TData>> propertySelector,
            Expression<Func<TData, TOrderBy>> orderBySelector = null,
            IEnumerable<string> includes = null)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));

            if (propertySelector == null)
            {
                throw new ArgumentNullException(nameof(propertySelector));
            }

            DbSet = propertySelector(Context) ?? throw new InvalidOperationException("propertySelector returned null. Expected a non null property.");
            OrderBySelector = orderBySelector;
            Includes = includes ?? new string[0];

            IRelationalEntityTypeAnnotations mapping = context.Model.FindEntityType(typeof(TData).FullName).Relational();
            Schema = mapping.Schema;
            TableName = mapping.TableName;
        }

        /// <summary>
        /// Gets the instance of the EntityFramework context.
        /// </summary>
        protected TContext Context { get; }

        /// <summary>
        /// Gets the instance of the DbSet.
        /// </summary>
        protected DbSet<TData> DbSet { get; }

        /// <summary>
        /// Gets the order by property selector.
        /// </summary>
        protected Expression<Func<TData, TOrderBy>> OrderBySelector { get; }

        /// <summary>
        /// Gets the properties to get while selecting data.
        /// </summary>
        protected IEnumerable<string> Includes { get; }

        /// <summary>
        /// Gets the database entity schema.
        /// </summary>
        protected string Schema { get; }

        /// <summary>
        /// Gets the database entity table name.
        /// </summary>
        protected string TableName { get; }

        /// <inheritdoc />
        public async Task<long> CountAsync(Expression<Func<TData, bool>> predicate)
        {
            return await DbSet.LongCountAsync(predicate);
        }

        /// <inheritdoc/>
        public async Task<TData> FindAsync(TKey key)
        {
            TData entity = await DbSet.FindAsync(key);

            if (entity != null)
            {
                foreach (string include in Includes)
                {
                    var reference = Context.Entry(entity).References.Where(r => r.Metadata.Name == include).FirstOrDefault();

                    if (reference != null)
                    {
                        await reference.LoadAsync();
                    }

                    var collection = Context.Entry(entity).Collections.Where(r => r.Metadata.Name == include).FirstOrDefault();

                    if (collection != null)
                    {
                        await collection.LoadAsync();
                    }
                }
            }

            return entity;
        }

        /// <inheritdoc/>
        public async Task<TData> FirstAsync(Expression<Func<TData, bool>> predicate)
        {
            return await PrepareQuery(predicate, 0, 1, false).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TData>> AnyAsync(long skip, long take, bool lastFirst)
        {
            return await Task.FromResult(PrepareQuery(null, skip, take, lastFirst).AsEnumerable());
        }

        /// <inheritdoc/>
        public async Task<TData> LastAsync(Expression<Func<TData, bool>> predicate)
        {
            return await PrepareQuery(predicate, 0, 1, false).LastOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TData>> QueryAsync(Expression<Func<TData, bool>> predicate, long skip, long take, bool lastFirst)
        {
            return await Task.FromResult(PrepareQuery(predicate, skip, take, lastFirst).AsEnumerable());
        }

        private IQueryable<TData> PrepareQuery(Expression<Func<TData, bool>> predicate, long skip, long take, bool lastFirst)
        {
            if (skip < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(skip));
            }

            if (take < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(take));
            }

            IQueryable<TData> query = DbSet.AsNoTracking();

            foreach (string include in Includes)
            {
                query = query.Include(include);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (lastFirst)
            {
                query = query.OrderByDescending(OrderBySelector);
            }
            else
            {
                query = query.OrderBy(OrderBySelector);
            }

            return query
                .Skip((int)skip)
                .Take((int)take);
        }
    }
}
