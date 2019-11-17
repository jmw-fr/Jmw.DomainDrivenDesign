// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Repositories.EntityFrameworkCore
{
    using System;
    using Jmw.DDD.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Basic implementation of <see cref="IRepository{TData, TKey}"/> for EntityFrameworkCore.
    /// </summary>
    /// <typeparam name="TContext">Entity DbContext class.</typeparam>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    [Obsolete("Please use TransactionalRepository.")]
    public abstract class Repository<TContext, TData, TKey> :
        TransactionalRepository<TContext, TData, TKey>,
        IRepository<TData, TKey>
        where TContext : DbContext
        where TData : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TContext, TData, TKey}"/> class.
        /// </summary>
        /// <param name="context">EntityFramework context to use.</param>
        /// <param name="propertySelector">Selector of the DbSet property of <paramref name="context"/>.</param>
        public Repository(
            TContext context,
            Func<TContext, DbSet<TData>> propertySelector)
            : base(context, propertySelector)
        {
        }
    }
}
