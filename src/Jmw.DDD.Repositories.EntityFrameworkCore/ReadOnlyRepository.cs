﻿// <copyright file="ReadOnlyRepository.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Jmw.DDD.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using NLog;

    /// <summary>
    /// Basic implementation of <see cref="IReadOnlyRepository{TData, TKey}"/> for EntityFrameworkCore.
    /// </summary>
    /// <typeparam name="TContext">Entity DbContext class.</typeparam>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    /// <typeparam name="TOrderBy">Order by property type.</typeparam>
    public abstract class ReadOnlyRepository<TContext, TData, TKey, TOrderBy> :
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
        public ReadOnlyRepository(
            TContext context,
            Func<TContext, DbSet<TData>> propertySelector)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));

            if (propertySelector == null)
            {
                throw new ArgumentNullException(nameof(propertySelector));
            }

            DbSet = propertySelector(Context) ?? throw new InvalidOperationException("propertySelector returned null. Expected a non null property.");

            IRelationalEntityTypeAnnotations mapping = context.Model.FindEntityType(typeof(TData).FullName).Relational();
            Schema = mapping.Schema;
            TableName = mapping.TableName;
        }

        /// <summary>
        /// Gets the order by properties.
        /// </summary>
        public IEnumerable<Expression<Func<TData, object>>> OrderBy { get; private set; }

        /// <summary>
        /// Gets the properties to retrieve when selecting data.
        /// </summary>
        public IEnumerable<string> Includes { get; private set; }

        /// <summary>
        /// Gets the instance of the EntityFramework context.
        /// </summary>
        protected internal TContext Context { get; }

        /// <summary>
        /// Gets the instance of the DbSet.
        /// </summary>
        protected internal DbSet<TData> DbSet { get; }

        /// <summary>
        /// Gets the database entity schema.
        /// </summary>
        protected internal string Schema { get; }

        /// <summary>
        /// Gets the database entity table name.
        /// </summary>
        protected internal string TableName { get; }

        /// <inheritdoc />
        public async Task<long> CountAsync(Expression<Func<TData, bool>> predicate = null)
        {
            Logger.Debug("ReadOnlyRepository::CountAsync");

            if (predicate is null)
            {
                return await DbSet.LongCountAsync();
            }
            else
            {
                return await DbSet.LongCountAsync(predicate);
            }
        }

        /// <inheritdoc/>
        public async Task<TData> FindAsync(TKey key)
        {
            Logger.Debug("ReadOnlyRepository::FindAsync");

            TData entity = await DbSet.FindAsync(key);

            if (entity != null && Includes != null)
            {
                foreach (var include in Includes)
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
        public async Task<TData> FirstAsync(Expression<Func<TData, bool>> predicate = null)
        {
            Logger.Debug("ReadOnlyRepository::FirstAsync");

            return await PrepareQuery(predicate, 0, 1).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<TData> LastAsync(Expression<Func<TData, bool>> predicate = null)
        {
            Logger.Debug("ReadOnlyRepository::LastAsync");

            return await PrepareQuery(predicate, 0, int.MaxValue).LastOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TData>> AnyAsync(long skip, long take)
        {
            Logger.Debug("ReadOnlyRepository::AnyAsync");

            return await Task.FromResult(PrepareQuery(null, skip, take).AsEnumerable());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TData>> QueryAsync(Expression<Func<TData, bool>> predicate, long skip, long take)
        {
            Logger.Debug("ReadOnlyRepository::QueryAsync");

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), "Use AnyAsync instead.");
            }

            return await Task.FromResult(PrepareQuery(predicate, skip, take).AsEnumerable());
        }

        /// <summary>
        /// Sets the order by properties for the repository.
        /// </summary>
        /// <param name="orderByProperties">List of properties used for ordering.</param>
        protected void SetOrderBy(params Expression<Func<TData, object>>[] orderByProperties)
        {
            OrderBy = orderByProperties;
        }

        /// <summary>
        /// Sets the references or collections to retrieve when selecting data.
        /// </summary>
        /// <param name="includesProperties">List of properties to include.</param>
        protected void SetIncludes(params Expression<Func<TData, object>>[] includesProperties)
        {
            Includes = includesProperties.Select(e => GetPropertyName(e)).ToList();
        }

        private static string GetPropertyName(Expression<Func<TData, object>> propertyLambda)
        {
            MemberExpression memberExpression = propertyLambda.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
            }

            return memberExpression.Member.Name;
        }

        private IQueryable<TData> PrepareQuery(Expression<Func<TData, bool>> predicate, long skip, long take)
        {
            bool ordered = false;

            // Entity Framework supports only int only until now.
            if (skip < 0 || skip > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(skip));
            }

            // Entity Framework supports only int only until now.
            if (take < 0 || take > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(take));
            }

            if (OrderBy is null)
            {
                throw new InvalidOperationException("Please set entities ordering with SetOrderBy() function first.");
            }

            IQueryable<TData> query = DbSet.AsNoTracking();

            if (Includes != null)
            {
                foreach (string include in Includes)
                {
                    query = query.Include(include);
                }
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (var order in OrderBy)
            {
                if (ordered)
                {
                    query = (query as IOrderedQueryable<TData>).ThenBy(order);
                }
                else
                {
                    query = query.OrderBy(order);
                    ordered = true;
                }
            }

            return query
                .Skip((int)skip)
                .Take((int)take);
        }
    }
}
