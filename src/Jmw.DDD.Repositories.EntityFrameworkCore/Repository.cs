// <copyright file="Repository.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Jmw.ComponentModel.DataAnnotations;
    using Jmw.DDD.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Basic implementation of <see cref="IRepository{TData, TKey}"/> for EntityFrameworkCore.
    /// </summary>
    /// <typeparam name="TContext">Entity DbContext class.</typeparam>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    /// <typeparam name="TOrderBy">Order by property type.</typeparam>
    public abstract class Repository<TContext, TData, TKey, TOrderBy> :
        ReadOnlyRepository<TContext, TData, TKey, TOrderBy>,
        IRepository<TData, TKey>
        where TContext : DbContext
        where TData : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TContext, TData, TKey, TOrderBy}"/> class.
        /// </summary>
        /// <param name="context">EntityFramework context to use.</param>
        /// <param name="propertySelector">Selector of the DbSet property of <paramref name="context"/>.</param>
        /// <param name="orderBySelector">Selector of <paramref name="propertySelector"/> property used to order by.</param>
        /// <param name="includes">Referenced properties to include during selection of data.</param>
        public Repository(
            TContext context,
            Func<TContext, DbSet<TData>> propertySelector,
            Expression<Func<TData, TOrderBy>> orderBySelector = null,
            IEnumerable<string> includes = null)
            : base(context, propertySelector, orderBySelector, includes)
        {
        }

        /// <inheritdoc />
        public async Task InsertAsync(TData entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.ValidateModel();

            DbSet.Add(entity);

            await Context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(TData entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.ValidateModel();

            DbSet.Update(entity);

            await Context.SaveChangesAsync();
        }
    }
}
