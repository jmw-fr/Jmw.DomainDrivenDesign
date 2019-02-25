// <copyright file="FindHandler.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Queries.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Jmw.ComponentModel.DataAnnotations;
    using Jmw.DDD.Domain.Repositories;
    using Jmw.DDD.Queries;
    using MediatR;
    using NLog;

    /// <summary>
    /// MediatR handler for <see cref="FindQuery{TReturn, TKey}"/>
    /// </summary>
    /// <typeparam name="TReturn">Returned element type.</typeparam>
    /// <typeparam name="TKey">Element id type.</typeparam>
    /// <typeparam name="TRepository">Repository type.</typeparam>
    /// <typeparam name="TEntity">Repository element type.</typeparam>
    public abstract class FindHandler<TReturn, TKey, TRepository, TEntity>
        : IRequestHandler<FindQuery<TReturn, TKey>, TReturn>
        where TReturn : class
        where TRepository : class, IReadOnlyRepository<TEntity, TKey>
        where TEntity : class
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly TRepository repository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindHandler{TReturn, TKey, TRepository, TEntity}"/> class.
        /// </summary>
        /// <param name="repository">Instance of the repository.</param>
        /// <param name="mapper">AutoMapper instance. The instance must be configured
        /// to map from <typeparamref name="TEntity"/> to <typeparamref name="TReturn"/>
        /// </param>
        /// <exception cref="ArgumentNullException">One of the parameter is <c>null</c>.</exception>
        public FindHandler(TRepository repository, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<TReturn> Handle(FindQuery<TReturn, TKey> request, CancellationToken cancellationToken)
        {
            try
            {
                request.ValidateModel();

                var result = await repository.FindAsync(request.Id);

                if (result != null)
                {
                    return mapper.Map<TReturn>(result)
                        ?? throw new InvalidOperationException($"Mapping {typeof(TEntity)} to {typeof(TReturn)}");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}
