// <copyright file="ReadOnlyRepository.cs" company="Jean-Marc Weeger">
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
    using Jmw.DDD.Domain.Repositories;
    using Jmw.DDD.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using NLog;

    /// <summary>
    /// Basic implementation of <see cref="IReadOnlyRepository{TData, TKey}"/> for EntityFrameworkCore.
    /// </summary>
    /// <typeparam name="TContext">Entity DbContext class.</typeparam>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    public abstract class ReadOnlyRepository<TContext, TData, TKey> :
        IReadOnlyRepository<TData, TKey>
        where TContext : DbContext
        where TData : class
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private IList<string> internalOrderBy;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepository{TContext, TData, TKey}"/> class.
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
        /// Gets the OrderBy properties.
        /// </summary>
        public IEnumerable<string> OrderBy => internalOrderBy;

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

            return await PrepareQuery(predicate, 0, 1, SortOrder.Ascending).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<TData> LastAsync(Expression<Func<TData, bool>> predicate = null)
        {
            Logger.Debug("ReadOnlyRepository::LastAsync");

            return await PrepareQuery(predicate, 0, 1, SortOrder.Descending).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TData>> AnyAsync(
            long skip,
            long take,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            Logger.Debug("ReadOnlyRepository::AnyAsync");

            return await Task.FromResult(PrepareQuery(null, skip, take, sortOrder).AsEnumerable());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TData>> QueryAsync(
            Expression<Func<TData, bool>> predicate,
            long skip,
            long take,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            Logger.Debug("ReadOnlyRepository::QueryAsync");

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), "Use AnyAsync instead.");
            }

            return await Task.FromResult(PrepareQuery(predicate, skip, take, sortOrder).AsEnumerable());
        }

        /// <summary>
        /// Sets the order by properties for the repository.
        /// </summary>
        /// <param name="orderByProperty">First property used to order.</param>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <returns>Return this.</returns>
        protected ReadOnlyRepository<TContext, TData, TKey> SetOrderBy<TProperty>(Expression<Func<TData, TProperty>> orderByProperty)
        {
            internalOrderBy = new List<string>()
            {
                GetPropertyName(orderByProperty),
            };

            return this;
        }

        /// <summary>
        /// Sets the order by properties for the repository.
        /// </summary>
        /// <param name="orderByProperty">Next properties used to order.</param>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <returns>Return this.</returns>
        protected ReadOnlyRepository<TContext, TData, TKey> ThenBy<TProperty>(Expression<Func<TData, TProperty>> orderByProperty)
        {
            if (internalOrderBy == null)
            {
                throw new InvalidOperationException("Call SetOrderBy() function first.");
            }

            internalOrderBy.Add(GetPropertyName(orderByProperty));

            return this;
        }

        /// <summary>
        /// Sets the references or collections to retrieve when selecting data.
        /// </summary>
        /// <param name="includesProperties">List of properties to include.</param>
        protected void SetIncludes(params Expression<Func<TData, object>>[] includesProperties)
        {
            Includes = includesProperties.Select(e => GetPropertyName(e)).ToList();
        }

        private static string GetPropertyName<TPropertyType>(Expression<Func<TData, TPropertyType>> propertyLambda)
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
