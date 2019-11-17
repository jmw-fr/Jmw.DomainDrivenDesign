// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.MediatR.UnitTest.Queries.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using AutoFixture;
    using AutoMapper;
    using Jmw.DDD.Application;
    using Jmw.DDD.Application.Repositories;
    using Jmw.DDD.Queries;
    using Jmw.DDD.Queries.Handlers;
    using Moq;

    /// <summary>
    /// Fixtures for <see cref="ListHandlerUnitTest"/>
    /// </summary>
    public class ListHandlerFixtures
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListHandlerFixtures"/> class.
        /// </summary>
        /// <param name="testType">Type of test to emulate.</param>
        public ListHandlerFixtures(TestTypeEnum testType)
        {
            var fixture = new Fixture();

            TestType = testType;

            ReadOnlyRepository = new Mock<IReadOnlyRepository<string, Guid>>();

            Mapper = new Mock<IMapper>();

            switch (TestType)
            {
                case TestTypeEnum.Ok:
                    {
                        Query = new ListQuery<string>(
                            fixture.Create<long>(),
                            fixture.Create<long>(),
                            fixture.Create<SortOrder>(),
                            fixture.Create<long>());
                        QueryResult = fixture.Create<IEnumerable<string>>();
                        Entities = fixture.Create<IEnumerable<string>>();
                        TotalElements = fixture.Create<long>();

                        ReadOnlyRepository.Setup(
                            f => f.AnyAsync(
                                It.Is<long>(m => m == Query.Skip),
                                It.Is<long>(m => m == Query.Take),
                                It.Is<SortOrder>(m => m == Query.SortOrder)))
                            .ReturnsAsync(Entities)
                            .Verifiable();

                        ReadOnlyRepository.Setup(
                            f => f.CountAsync(It.Is<Expression<Func<string, bool>>>(m => m == null)))
                            .ReturnsAsync(TotalElements)
                            .Verifiable();

                        Mapper.Setup(
                            f => f.Map<IEnumerable<string>>(It.Is<IEnumerable<string>>(m => m == Entities)))
                            .Returns(QueryResult)
                            .Verifiable();

                        break;
                    }

                case TestTypeEnum.RepositoryThrowsInvalidOperationException:
                    {
                        Query = new ListQuery<string>(
                            fixture.Create<long>(),
                            fixture.Create<long>(),
                            fixture.Create<SortOrder>(),
                            fixture.Create<long>());

                        ReadOnlyRepository.Setup(
                            f => f.AnyAsync(
                                It.Is<long>(m => m == Query.Skip),
                                It.Is<long>(m => m == Query.Take),
                                It.Is<SortOrder>(m => m == Query.SortOrder)))
                            .Throws<InvalidOperationException>()
                            .Verifiable();

                        break;
                    }

                case TestTypeEnum.MapReturnsNull:
                    {
                        Query = new ListQuery<string>(
                            fixture.Create<long>(),
                            fixture.Create<long>(),
                            fixture.Create<SortOrder>(),
                            fixture.Create<long>());
                        QueryResult = fixture.Create<IEnumerable<string>>();
                        Entities = fixture.Create<IEnumerable<string>>();
                        TotalElements = fixture.Create<long>();

                        ReadOnlyRepository.Setup(
                            f => f.AnyAsync(
                                It.Is<long>(m => m == Query.Skip),
                                It.Is<long>(m => m == Query.Take),
                                It.Is<SortOrder>(m => m == Query.SortOrder)))
                            .ReturnsAsync(Entities)
                            .Verifiable();

                        Mapper.Setup(
                            f => f.Map<IEnumerable<string>>(It.Is<IEnumerable<string>>(m => m == Entities)))
                            .Returns(null as IEnumerable<string>)
                            .Verifiable();

                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException($"Unknown test type: {testType}");
                    }
            }
        }

        /// <summary>
        /// Type of test.
        /// </summary>
        public enum TestTypeEnum
        {
            /// <summary>
            /// Simulate a test without error.
            /// </summary>
            Ok,

            /// <summary>
            /// The repository throws an exception.
            /// </summary>
            RepositoryThrowsInvalidOperationException,

            /// <summary>
            /// The mapping with AutoMapper returns null.
            /// </summary>
            MapReturnsNull,
        }

        /// <summary>
        /// Gets the type of test to emulate.
        /// </summary>
        public TestTypeEnum TestType { get; }

        /// <summary>
        /// Gets the query fixture.
        /// </summary>
        public ListQuery<string> Query { get; }

        /// <summary>
        /// Gets the entity resulting from the repository.
        /// </summary>
        public IEnumerable<string> Entities { get; }

        /// <summary>
        /// Gets the result of the query.
        /// </summary>
        public IEnumerable<string> QueryResult { get; }

        /// <summary>
        /// Gets the total elements in the repository.
        /// </summary>
        public long TotalElements { get; }

        /// <summary>
        /// Gets the repository mock.
        /// </summary>
        public Mock<IReadOnlyRepository<string, Guid>> ReadOnlyRepository { get; }

        /// <summary>
        /// Gets the AutoMapper instance.
        /// </summary>
        public Mock<IMapper> Mapper { get; }

        /// <summary>
        /// Mock for <see cref="ListHandler{TReturn, TKey, TRepository, TEntity}"/>
        /// </summary>
        public class ListHandlerMock : ListHandler<string, Guid, IReadOnlyRepository<string, Guid>, string>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ListHandlerMock"/> class.
            /// </summary>
            /// <param name="repository">Repository instance.</param>
            /// <param name="mapper">AutoMapper instance.</param>
            public ListHandlerMock(IReadOnlyRepository<string, Guid> repository, IMapper mapper)
                : base(repository, mapper)
            {
            }
        }
    }
}
