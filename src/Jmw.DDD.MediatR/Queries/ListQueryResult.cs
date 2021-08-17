// <copyright file="ListQueryResult.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Queries
{
    using System.Collections.Generic;
    using Jmw.DDD.Domain;

    /// <summary>
    /// Result from <see cref="ListQuery{TReturn}"/>.
    /// </summary>
    /// <typeparam name="T">Type de la réponse.</typeparam>
    public class ListQueryResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListQueryResult{T}"/> class.
        /// </summary>
        /// <param name="data">Data returned.</param>
        /// <param name="totalElements">Number of elements of the query.</param>
        /// <param name="skip">Number of elements skipped in the query result.</param>
        /// <param name="take">Number of elements that should be returned.</param>
        /// <param name="sortOrder">Sort order of the data to retrieve.</param>
        /// <param name="draw">Request number.</param>
        public ListQueryResult(
            IEnumerable<T> data,
            long totalElements,
            long skip,
            long take,
            SortOrder sortOrder,
            long draw)
        {
            Data = data;
            TotalElements = totalElements;
            Skip = skip;
            Take = take;
            SortOrder = sortOrder;
            Draw = draw;
        }

        /// <summary>
        /// Gets the request number from <see cref="ListQueryResult{T}.Draw" />.
        /// </summary>
        public long Draw { get; }

        /// <summary>
        /// Gets the data result.
        /// </summary>
        public IEnumerable<T> Data { get; }

        /// <summary>
        /// Gets the total number of elements corresponding to the request.
        /// </summary>
        public long TotalElements { get; }

        /// <summary>
        /// Gets the number of elements skipped in the query result.
        /// </summary>
        public long Skip { get; }

        /// <summary>
        /// Gets the number of elements that should be returned.
        /// </summary>
        public long Take { get; }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        public SortOrder SortOrder { get; }
    }
}
