// <copyright file="ListQuery.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Queries
{
    using System.ComponentModel.DataAnnotations;
    using Jmw.DDD.Domain;
    using MediatR;

    /// <summary>
    /// List for data without filter.
    /// </summary>
    /// <typeparam name="TReturn">Type de retour de la query.</typeparam>
    public class ListQuery<TReturn> : IRequest<ListQueryResult<TReturn>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListQuery{TReturn}"/> class.
        /// </summary>
        /// <param name="skip">Number of elements skipped in the query result.</param>
        /// <param name="take">Number of elements that should be returned.</param>
        /// <param name="sortOrder">Sort order of the data to retrieve.</param>
        /// <param name="draw">Request number.</param>
        public ListQuery(long skip, long take, SortOrder sortOrder, long draw)
        {
            Skip = skip;
            Take = take;
            SortOrder = sortOrder;
            Draw = draw;
        }

        /// <summary>
        /// Gets the request number.
        /// This value can be used by the client to ignore
        /// an answer if it does not match its last request.
        /// </summary>
        public long Draw { get; }

        /// <summary>
        /// Gets the number of elements skipped in the query result.
        /// </summary>
        [Range(0, long.MaxValue)]
        public long Skip { get; }

        /// <summary>
        /// Gets the number of elements that should be returned.
        /// </summary>
        [Range(0, long.MaxValue)]
        public long Take { get; }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        public SortOrder SortOrder { get; }
    }
}
