// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Domain.Repositories
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a repository that accept transactional Start / Commit / Rollback functions.
    /// </summary>
    /// <typeparam name="TData">Repository entity data type.</typeparam>
    /// <typeparam name="TKey">Repository key type.</typeparam>
    [Obsolete("Please use Jmw.DDD.Application.Repositories.ITransactionalRepository.")]
    public interface ITransactionalRepository<TData, TKey> :
        Application.Repositories.ITransactionalRepository<TData, TKey>
        where TData : class
    {
    }
}
