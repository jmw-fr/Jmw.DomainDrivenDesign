// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Repositories.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Jmw.DDD.Application;
    using Jmw.DDD.Application.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Basic implementation of <see cref="IReadOnlyRepository{TData, TKey}"/> for EntityFrameworkCore.
    /// </summary>
    /// <typeparam name="TContext">Entity DbContext class.</typeparam>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    [Obsolete("Please use TransactionalRepository.")]
    public abstract class ReadOnlyRepository<TContext, TData, TKey> :
        TransactionalRepository<TContext, TData, TKey>,
        Domain.Repositories.IReadOnlyRepository<TData, TKey>
        where TContext : DbContext
        where TData : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepository{TContext, TData, TKey}"/> class.
        /// </summary>
        /// <param name="context">EntityFramework context to use.</param>
        /// <param name="propertySelector">Selector of the DbSet property of <paramref name="context"/>.</param>
        public ReadOnlyRepository(TContext context, Func<TContext, DbSet<TData>> propertySelector)
            : base(context, propertySelector)
        {
        }

        /// <inheritdoc/>
        [Obsolete("Pleasure the function using Application.SortOrder.")]
        public Task<IEnumerable<TData>> QueryAsync(Expression<Func<TData, bool>> predicate, long skip, long take, Domain.SortOrder sortOrder = Domain.SortOrder.Ascending)
        {
            return QueryAsync(predicate, skip, take, (SortOrder)sortOrder);
        }

        /// <inheritdoc/>
        [Obsolete("Pleasure the function using Application.SortOrder.")]
        public Task<IEnumerable<TData>> AnyAsync(long skip, long take, Domain.SortOrder sortOrder = Domain.SortOrder.Ascending)
        {
            return AnyAsync(skip, take, (SortOrder)sortOrder);
        }
    }
}
