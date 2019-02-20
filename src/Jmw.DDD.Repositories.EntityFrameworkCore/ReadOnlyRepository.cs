// <copyright file="ReadOnlyRepository.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCore
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Jmw.DDD.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Basic implementation of <see cref="IReadOnlyRepository{TData, TKey}"/> for EntityFrameworkCore.
    /// </summary>
    /// <typeparam name="TContext">Entity DbContext class.</typeparam>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    public abstract class ReadOnlyRepository<TContext, TData, TKey> :
        RepositoryBase<TContext, TData>,
        IReadOnlyRepository<TData, TKey>
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
        public async Task<TData> FindAsync(TKey key)
        {
            Logger.Debug("ReadOnlyRepository::FindAsync");

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
    }
}
