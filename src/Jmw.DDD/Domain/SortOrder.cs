﻿// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Domain
{
    using System;

    /// <summary>
    /// Sort order for queries.
    /// </summary>
    [Obsolete("Please use Jmw.DDD.Application.Repositories.SortOrder.")]
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
