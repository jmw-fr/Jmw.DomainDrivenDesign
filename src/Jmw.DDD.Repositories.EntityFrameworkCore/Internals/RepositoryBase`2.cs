// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Repositories.EntityFrameworkCore
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Jmw.DDD.Application;
    using Jmw.DDD.Application.Repositories;
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
        /// From https://stackoverflow.com/questions/31955025/generate-ef-orderby-expression-by-string :)
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <param name="query">Source query.</param>
        /// <param name="methodName">Order by method to use.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Ordered query.</returns>
        protected static IOrderedQueryable<TSource> MakeOrderExpression<TSource>(
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
        /// Prepare an Entity query.
        /// </summary>
        /// <param name="predicate">Predicate function.</param>
        /// <param name="skip">Number of elements to skip.</param>
        /// <param name="take">Number of elements to return.</param>
        /// <param name="sortOrder">Sort order of the elements.</param>
        /// <returns>Queryable object.</returns>
        protected IQueryable<TData> PrepareQuery(
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

            IQueryable<TData> query = Configuration.DbSet;

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
