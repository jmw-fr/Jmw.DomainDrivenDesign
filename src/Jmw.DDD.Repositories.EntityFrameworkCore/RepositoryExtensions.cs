// <copyright file="RepositoryExtensions.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.EntityFrameworkCore
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Extensions for Entity Repository.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Indicates the sort order of the entities in a repository.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <param name="sortProperties">Properties used for sorting.</param>
        public static void SortBy<TEntity>(params Expression<Func<TEntity, object>>[] sortProperties)
        {
        }
    }
}
