// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.MediatR.UnitTest.Queries.Handlers
{
    using System;
    using AutoFixture;
    using AutoMapper;
    using Jmw.DDD.Application.Repositories;
    using Jmw.DDD.Queries;
    using Jmw.DDD.Queries.Handlers;
    using Moq;

    /// <summary>
    /// Fixtures for <see cref="FindHandlerUnitTest"/>
    /// </summary>
    public class FindHandlerFixtures
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindHandlerFixtures"/> class.
        /// </summary>
        /// <param name="testType">Type of test to emulate.</param>
        public FindHandlerFixtures(TestTypeEnum testType)
        {
            var fixture = new Fixture();

            TestType = testType;

            ReadOnlyRepository = new Mock<IReadOnlyRepository<string, Guid>>();

            Mapper = new Mock<IMapper>();

            switch (TestType)
            {
                case TestTypeEnum.Ok:
                    {
                        Query = new FindQuery<string, Guid>(fixture.Create<Guid>());
                        QueryResult = fixture.Create<string>();
                        Entity = fixture.Create<string>();

                        ReadOnlyRepository.Setup(
                            f => f.FindAsync(It.Is<Guid>(m => m == Query.Id)))
                            .ReturnsAsync(Entity)
                            .Verifiable();

                        Mapper.Setup(
                        f => f.Map<string>(It.Is<string>(m => m == Entity)))
                        .Returns(QueryResult)
                        .Verifiable();

                        break;
                    }

                case TestTypeEnum.RepositoryThrowsInvalidOperationException:
                    {
                        Query = new FindQuery<string, Guid>(fixture.Create<Guid>());

                        ReadOnlyRepository.Setup(
                            f => f.FindAsync(It.Is<Guid>(m => m == Query.Id)))
                            .Throws<InvalidOperationException>()
                            .Verifiable();

                        break;
                    }

                case TestTypeEnum.MapReturnsNull:
                    {
                        Query = new FindQuery<string, Guid>(fixture.Create<Guid>());
                        QueryResult = fixture.Create<string>();
                        Entity = fixture.Create<string>();

                        ReadOnlyRepository.Setup(
                            f => f.FindAsync(It.Is<Guid>(m => m == Query.Id)))
                            .ReturnsAsync(Entity)
                            .Verifiable();

                        Mapper.Setup(
                        f => f.Map<string>(It.Is<string>(m => m == Entity)))
                        .Returns(null as string)
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
        public FindQuery<string, Guid> Query { get; }

        /// <summary>
        /// Gets the entity resulting from the repository.
        /// </summary>
        public string Entity { get; }

        /// <summary>
        /// Gets the result of the query.
        /// </summary>
        public string QueryResult { get; }

        /// <summary>
        /// Gets the repository mock.
        /// </summary>
        public Mock<IReadOnlyRepository<string, Guid>> ReadOnlyRepository { get; }

        /// <summary>
        /// Gets the AutoMapper instance.
        /// </summary>
        public Mock<IMapper> Mapper { get; }

        /// <summary>
        /// Mock for <see cref="FindQuery{TReturn, TKey}"/>
        /// </summary>
        public class FindHandlerMock : FindHandler<string, Guid, IReadOnlyRepository<string, Guid>, string>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FindHandlerMock"/> class.
            /// </summary>
            /// <param name="repository">Repository instance.</param>
            /// <param name="mapper">AutoMapper instance.</param>
            public FindHandlerMock(IReadOnlyRepository<string, Guid> repository, IMapper mapper)
                : base(repository, mapper)
            {
            }
        }
    }
}
