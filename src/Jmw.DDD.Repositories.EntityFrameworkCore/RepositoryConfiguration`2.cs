// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Repositories.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    /// <summary>
    /// Class used to configure the repositories.
    /// </summary>
    /// <typeparam name="TContext">Entity DbContext class.</typeparam>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    public class RepositoryConfiguration<TContext, TData>
        where TContext : DbContext
        where TData : class
    {
        private IList<string> internalOrderBy;

        private IList<string> internalIncludes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryConfiguration{TContext, TData}"/> class.
        /// </summary>
        /// <param name="context">EntityFramework context to use.</param>
        /// <param name="propertySelector">Selector of the DbSet property of <paramref name="context"/>.</param>
        internal RepositoryConfiguration(
            TContext context,
            Func<TContext, DbSet<TData>> propertySelector)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));

            if (propertySelector == null)
            {
                throw new ArgumentNullException(nameof(propertySelector));
            }

            DbSet = propertySelector(Context)
                ?? throw new InvalidOperationException("propertySelector returned null. Expected a non null property.");

            internalIncludes = new List<string>();
            internalOrderBy = new List<string>();

            IRelationalEntityTypeAnnotations mapping = context.Model.FindEntityType(typeof(TData).FullName).Relational();
            Schema = mapping.Schema;
            TableName = mapping.TableName;
        }

        /// <summary>
        /// Gets the OrderBy properties.
        /// </summary>
        public IEnumerable<string> OrderByProperties => internalOrderBy;

        /// <summary>
        /// Gets the additional properties to retrieve when selecting data.
        /// </summary>
        public IEnumerable<string> IncludeProperties => internalIncludes;

        /// <summary>
        /// Gets the database entity schema.
        /// </summary>
        public string Schema { get; }

        /// <summary>
        /// Gets the database entity table name.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets the instance of the EntityFramework context.
        /// </summary>
        public TContext Context { get; }

        /// <summary>
        /// Gets the instance of the DbSet.
        /// </summary>
        public DbSet<TData> DbSet { get; }

        /// <summary>
        /// Sets the order by properties for the repository.
        /// </summary>
        /// <param name="orderByProperty">First property used to order.</param>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <returns>Return this.</returns>
        public RepositoryConfiguration<TContext, TData> OrderBy<TProperty>(Expression<Func<TData, TProperty>> orderByProperty)
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
        public RepositoryConfiguration<TContext, TData> ThenBy<TProperty>(Expression<Func<TData, TProperty>> orderByProperty)
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
        /// <param name="include">Property to include.</param>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <returns>Return this.</returns>
        public RepositoryConfiguration<TContext, TData> Include<TProperty>(Expression<Func<TData, TProperty>> include)
        {
            internalIncludes.Add(GetPropertyName(include));

            return this;
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
    }
}
