// <copyright file="ListQueryResultUnitTest.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.MediatR.UnitTest.Queries
{
    using System.Collections.Generic;
    using AutoFixture;
    using Jmw.DDD.Domain;
    using Jmw.DDD.Queries;
    using Xunit;

    /// <summary>
    /// <see cref="ListQueryResult{T}"/> unit tests.
    /// </summary>
    public class ListQueryResultUnitTest
    {
        /// <summary>
        /// Tests that <see cref="ListQueryResult{T}" /> correctly set properties.
        /// </summary>
        [Fact]
        [Trait("MediatR", "ListQueryResult")]
        public void ListQueryResult_MustSet_Properties()
        {
            // Arrange
            var fixture = new Fixture();
            var data = fixture.Create<IEnumerable<string>>();
            var totalElements = fixture.Create<long>();
            var skip = fixture.Create<long>();
            var take = fixture.Create<long>();
            var sortOrder = fixture.Create<SortOrder>();
            var draw = fixture.Create<long>();

            // Act
            var sut = new ListQueryResult<string>(data, totalElements, skip, take, sortOrder, draw);

            // Assert
            Assert.Equal(data, sut.Data);
            Assert.Equal(totalElements, sut.TotalElements);
            Assert.Equal(skip, sut.Skip);
            Assert.Equal(take, sut.Take);
            Assert.Equal(sortOrder, sut.SortOrder);
            Assert.Equal(draw, sut.Draw);
        }
    }
}
