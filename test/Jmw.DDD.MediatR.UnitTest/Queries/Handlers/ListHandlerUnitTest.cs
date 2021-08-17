// <copyright file="ListHandlerUnitTest.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.MediatR.UnitTest.Queries.Handlers
{
    using System;
    using Jmw.DDD.Queries.Handlers;
    using Xunit;

    /// <summary>
    /// <see cref="ListHandler{TReturn, TKey, TRepository, TEntity}"/> unit tests.
    /// </summary>
    public class ListHandlerUnitTest
    {
        /// <summary>
        /// Checks that the constructor correctly throws the exceptions.
        /// </summary>
        [Fact]
        [Trait("MediatR", "ListHandler")]
        public void Constructor_Must_ThrowExceptions()
        {
            // Arrange
            var fixture = new ListHandlerFixtures(ListHandlerFixtures.TestTypeEnum.Ok);
            Action sut1 = () => new ListHandlerFixtures.ListHandlerMock(
                null, fixture.Mapper.Object);
            Action sut2 = () => new ListHandlerFixtures.ListHandlerMock(
                fixture.ReadOnlyRepository.Object, null);

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(sut1);
            Assert.Throws<ArgumentNullException>(sut2);
        }

        /// <summary>
        /// Checks that the handler returns the correct result.
        /// </summary>
        [Fact]
        [Trait("MediatR", "ListHandler")]
        public async void Handler_MustReturn_Result()
        {
            // Arrange
            var fixture = new ListHandlerFixtures(ListHandlerFixtures.TestTypeEnum.Ok);
            var query = fixture.Query;
            var sut = new ListHandlerFixtures.ListHandlerMock(
                fixture.ReadOnlyRepository.Object,
                fixture.Mapper.Object);

            // Act
            var computed = await sut.Handle(query, default);

            // Assert
            Assert.NotNull(computed);
            Assert.Equal(fixture.QueryResult, computed.Data);
            Assert.Equal(fixture.TotalElements, computed.TotalElements);
            Assert.Equal(fixture.Query.Draw, computed.Draw);
            Assert.Equal(fixture.Query.Skip, computed.Skip);
            Assert.Equal(fixture.Query.Take, computed.Take);

            fixture.ReadOnlyRepository.VerifyAll();
            fixture.ReadOnlyRepository.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Checks that the handler correctly throws and manage the exception.
        /// </summary>
        /// <param name="testType">Test to perform.</param>
        /// <param name="typeException">Type of expected exception.</param>
        [Theory]
        [Trait("MediatR", "ListHandler")]
        [InlineData(ListHandlerFixtures.TestTypeEnum.RepositoryThrowsInvalidOperationException, typeof(InvalidOperationException))]
        [InlineData(ListHandlerFixtures.TestTypeEnum.MapReturnsNull, typeof(InvalidOperationException))]
        public async void Handler_Must_ThrowException(ListHandlerFixtures.TestTypeEnum testType, Type typeException)
        {
            // Arrange
            var fixture = new ListHandlerFixtures(testType);
            var query = fixture.Query;
            var sut = new ListHandlerFixtures.ListHandlerMock(
                fixture.ReadOnlyRepository.Object,
                fixture.Mapper.Object);

            // Act
            Exception ex = await Assert.ThrowsAnyAsync<Exception>(async () => await sut.Handle(query, default));

            // Assert
            Assert.Equal(typeException, ex.GetType());
            fixture.ReadOnlyRepository.VerifyAll();
            fixture.ReadOnlyRepository.VerifyNoOtherCalls();
        }
    }
}
