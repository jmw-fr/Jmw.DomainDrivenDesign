// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Domain.Repositories
{
    using System;

    /// <summary>
    /// Interface de base d'un repository.
    /// </summary>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    [Obsolete("Please use Jmw.DDD.Application.Repositories.")]
    public interface IRepository<TData, TKey> :
        Application.Repositories.IReadOnlyRepository<TData, TKey>,
        Application.Repositories.IWriteOnlyRepository<TData, TKey>
        where TData : class
    {
    }
}
