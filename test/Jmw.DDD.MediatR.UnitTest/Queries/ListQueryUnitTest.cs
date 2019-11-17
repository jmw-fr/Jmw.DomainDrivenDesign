// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.MediatR.UnitTest.Queries
{
    using AutoFixture;
    using Jmw.DDD.Application;
    using Jmw.DDD.Queries;
    using Xunit;

    /// <summary>
    /// <see cref="ListQuery{T}"/> unit tests.
    /// </summary>
    public class ListQueryUnitTest
    {
        /// <summary>
        /// Tests that <see cref="ListQuery{T}" /> correctly set properties.
        /// </summary>
        [Fact]
        [Trait("MediatR", "ListQuery")]
        public void ListQuery_MustSet_Properties()
        {
            // Arrange
            var fixture = new Fixture();
            var skip = fixture.Create<long>();
            var take = fixture.Create<long>();
            var sortOrder = fixture.Create<SortOrder>();
            var draw = fixture.Create<long>();

            // Act
            var sut = new ListQuery<string>(skip, take, sortOrder, draw);

            // Assert
            Assert.Equal(skip, sut.Skip);
            Assert.Equal(take, sut.Take);
            Assert.Equal(sortOrder, sut.SortOrder);
            Assert.Equal(draw, sut.Draw);
        }
    }
}
