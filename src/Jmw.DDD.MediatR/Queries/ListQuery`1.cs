// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Queries
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Jmw.DDD.Application;
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
        [Obsolete("Please use the constructor with Application.Repositories.SortOrder")]
        public ListQuery(long skip, long take, Domain.SortOrder sortOrder, long draw)
        {
            Skip = skip;
            Take = take;
            SortOrder = (SortOrder)sortOrder;
            Draw = draw;
        }

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
