// <copyright file="FindQuery.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Queries
{
    using System.ComponentModel.DataAnnotations;
    using MediatR;

    /// <summary>
    /// Request an element by Id.
    /// </summary>
    /// <typeparam name="TReturn">Element type.</typeparam>
    /// <typeparam name="TKey">Element id type.</typeparam>
    public class FindQuery<TReturn, TKey> : IRequest<TReturn>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindQuery{TReturn, TKey}"/> class.
        /// </summary>
        /// <param name="id">Element id requested.</param>
        public FindQuery(TKey id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the element id requested.
        /// </summary>
        [Required]
        public TKey Id { get; }
    }
}
