// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Queries.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Jmw.ComponentModel.DataAnnotations;
    using Jmw.DDD.Application.Repositories;
    using Jmw.DDD.Queries;
    using MediatR;
    using NLog;

    /// <summary>
    /// MediatR handler for <see cref="ListQuery{TReturn}"/>
    /// </summary>
    /// <typeparam name="TReturn">Returned element type.</typeparam>
    /// <typeparam name="TKey">Element id type.</typeparam>
    /// <typeparam name="TRepository">Repository type.</typeparam>
    /// <typeparam name="TEntity">Repository element type.</typeparam>
    public abstract class ListHandler<TReturn, TKey, TRepository, TEntity>
        : IRequestHandler<ListQuery<TReturn>, ListQueryResult<TReturn>>
        where TReturn : class
        where TRepository : class, IReadOnlyRepository<TEntity, TKey>
        where TEntity : class
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly TRepository repository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListHandler{TReturn, TKey, TRepository, TEntity}"/> class.
        /// </summary>
        /// <param name="repository">Instance of the repository.</param>
        /// <param name="mapper">AutoMapper instance. The instance must be configured
        /// to map from <typeparamref name="TEntity"/> to <typeparamref name="TReturn"/>
        /// </param>
        /// <exception cref="ArgumentNullException">One of the parameter is <c>null</c>.</exception>
        public ListHandler(TRepository repository, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<ListQueryResult<TReturn>> Handle(ListQuery<TReturn> request, CancellationToken cancellationToken)
        {
            try
            {
                request.ValidateModel();

                var entities = await repository.AnyAsync(
                                            request.Skip,
                                            request.Take,
                                            request.SortOrder);

                var data = mapper.Map<IEnumerable<TReturn>>(entities)
                    ?? throw new InvalidOperationException($"Mapping {typeof(TEntity)} to {typeof(TReturn)}");

                var totalElements = await repository.CountAsync();

                return new ListQueryResult<TReturn>(
                    data,
                    totalElements,
                    request.Skip,
                    request.Take,
                    request.SortOrder,
                    request.Draw);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}
