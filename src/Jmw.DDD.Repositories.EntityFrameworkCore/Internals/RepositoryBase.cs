// <copyright file="RepositoryBase.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Jmw.DDD.Domain;
    using Jmw.DDD.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;
    using NLog;

    /// <summary>
    /// Basic implementation of <see cref="IReadOnlyRepository{TData, TKey}"/> for EntityFrameworkCore.
    /// </summary>
    /// <typeparam name="TContext">Entity DbContext class.</typeparam>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    public abstract class RepositoryBase<TContext, TData>
        where TContext : DbContext
        where TData : class
    {
        /// <summary>
        /// NLog instance.
        /// </summary>
        protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly RepositoryConfiguration<TContext, TData> configuration;

        private bool configured;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TContext, TData}"/> class.
        /// </summary>
        /// <param name="context">EntityFramework context to use.</param>
        /// <param name="propertySelector">Selector of the DbSet property of <paramref name="context"/>.</param>
        internal RepositoryBase(
            TContext context,
            Func<TContext, DbSet<TData>> propertySelector)
        {
            configuration = new RepositoryConfiguration<TContext, TData>(context, propertySelector);
        }

        /// <summary>
        /// Gets the Repository configuration.
        /// </summary>
        public RepositoryConfiguration<TContext, TData> Configuration
        {
            get
            {
                lock (this)
                {
                    if (!configured)
                    {
                        OnConfigure(configuration);

                        configured = true;
                    }
                }

                return configuration;
            }
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
            Logger.Debug("RepositoryBase::CountAsync");

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
            Logger.Debug("RepositoryBase::FirstAsync");

            return await PrepareQuery(predicate, 0, 1, SortOrder.Ascending).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Implementation of <see cref="IReadOnlyRepository{TData, TKey}.LastAsync"/>.
        /// </summary>
        /// <param name="predicate">Optional predicate function.</param>
        /// <returns>Found data or <c>null</c>.</returns>
        public async Task<TData> LastAsync(Expression<Func<TData, bool>> predicate = null)
        {
            Logger.Debug("RepositoryBase::LastAsync");

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
            Logger.Debug("RepositoryBase::AnyAsync");

            return await Task.FromResult(PrepareQuery(null, skip, take, sortOrder).AsEnumerable());
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
            Logger.Debug("RepositoryBase::QueryAsync");

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), "Use AnyAsync instead.");
            }

            return await Task.FromResult(PrepareQuery(predicate, skip, take, sortOrder).AsEnumerable());
        }

        /// <summary>
        /// <para>
        /// Function called to override default configuration.
        /// </para>
        /// <para>
        /// Override this function to setup repository OrderBy and includes.
        /// </para>
        /// </summary>
        /// <param name="configuration">Repository configuration.</param>
        protected virtual void OnConfigure(RepositoryConfiguration<TContext, TData> configuration)
        {
        }

        /// <summary>
        /// From https://stackoverflow.com/questions/31955025/generate-ef-orderby-expression-by-string :)
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <param name="query">Source query.</param>
        /// <param name="methodName">Order by method to use.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Ordered query.</returns>
        private static IOrderedQueryable<TSource> MakeOrderExpression<TSource>(
            IQueryable<TSource> query,
            string methodName,
            string propertyName)
        {
            var entityType = typeof(TSource);

            // Create x=>x.PropName
            var propertyInfo = entityType.GetProperty(propertyName);
            ParameterExpression arg = Expression.Parameter(entityType, "x");
            MemberExpression property = Expression.Property(arg, propertyName);
            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

            // Get System.Linq.Queryable.OrderBy() method.
            var enumarableType = typeof(Queryable);
            var method = enumarableType.GetMethods()
                 .Where(m => m.Name == methodName && m.IsGenericMethodDefinition)
                 .Where(m =>
                 {
                     var parameters = m.GetParameters().ToList();

                     // Put more restriction here to ensure selecting the right overload
                     return parameters.Count == 2; // overload that has 2 parameters
                 }).Single();

            // The linq's OrderBy<TSource, TKey> has two generic types, which provided here
            MethodInfo genericMethod = method
                    .MakeGenericMethod(entityType, propertyInfo.PropertyType);

            /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
              Note that we pass the selector as Expression to the method and we don't compile it.
              By doing so EF can extract "order by" columns and generate SQL for it.*/
            var newQuery = (IOrderedQueryable<TSource>)genericMethod
                 .Invoke(genericMethod, new object[] { query, selector });

            return newQuery;
        }

        private IQueryable<TData> PrepareQuery(
            Expression<Func<TData, bool>> predicate,
            long skip,
            long take,
            SortOrder sortOrder = SortOrder.Ascending)
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

            if (!Enum.IsDefined(typeof(SortOrder), sortOrder))
            {
                throw new ArgumentOutOfRangeException(nameof(sortOrder));
            }

            IQueryable<TData> query = Configuration.DbSet.AsNoTracking();

            foreach (string include in Configuration.IncludeProperties)
            {
                query = query.Include(include);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (var order in Configuration.OrderByProperties)
            {
                if (ordered)
                {
                    query = MakeOrderExpression(query, sortOrder == SortOrder.Ascending ? "ThenBy" : "ThenByDescending", order);
                }
                else
                {
                    query = MakeOrderExpression(query, sortOrder == SortOrder.Ascending ? "OrderBy" : "OrderByDescending", order);
                    ordered = true;
                }
            }

            return query
                .Skip((int)skip)
                .Take((int)take);
        }
    }
}
