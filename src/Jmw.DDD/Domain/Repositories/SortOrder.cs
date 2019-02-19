// <copyright file="SortOrder.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Domain.Repositories
{
    /// <summary>
    /// Sort order for queries.
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Ascending sort.
        /// </summary>
        Ascending = 0,

        /// <summary>
        /// Descending sort.
        /// </summary>
        Descending = 1,
    }
}
