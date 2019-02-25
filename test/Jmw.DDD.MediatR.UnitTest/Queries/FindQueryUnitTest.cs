// <copyright file="FindQueryUnitTest.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.MediatR.UnitTest.Queries
{
    using System;
    using AutoFixture;
    using Jmw.DDD.Queries;
    using Xunit;

    /// <summary>
    /// <see cref="FindQuery{TReturn, TKey}"/> unit tests.
    /// </summary>
    public class FindQueryUnitTest
    {
        /// <summary>
        /// Tests that <see cref="FindQuery{TReturn, TKey}" /> correctly set properties.
        /// </summary>
        [Fact]
        [Trait("MediatR", "FindQuery")]
        public void FindQuery_MustSet_Properties()
        {
            // Arrange
            var fixture = new Fixture();
            var id = fixture.Create<Guid>();

            // Act
            var sut = new FindQuery<string, Guid>(id);

            // Assert
            Assert.Equal(id, sut.Id);
        }
    }
}
