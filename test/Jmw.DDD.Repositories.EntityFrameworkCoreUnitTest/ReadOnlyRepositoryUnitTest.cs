// <copyright file="ReadOnlyRepositoryUnitTest.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest
{
    using System;
    using Jmw.DDD.Repositories.EntityFrameworkCore;
    using Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common;
    using Xunit;

    /// <summary>
    /// <see cref="ReadOnlyRepository{TContext, TData, TKey, TOrderBy}"/> unit tests.
    /// </summary>
    public class ReadOnlyRepositoryUnitTest
    {
        /// <summary>
        /// Checks that the constructor correctly throws the exceptions.
        /// </summary>
        [Fact]
        [Trait("Repositories", "ReadOnlyRepository")]
        public void Constructor_Must_ThrowExceptions()
        {
            // Arrange
            Action sut1 = () => new RepositoryFixture(new DbContextFixture(), null);
            Action sut2 = () => new RepositoryFixture(null, c => c.TestData);

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(sut1);
            Assert.Throws<ArgumentNullException>(sut2);
        }
    }
}
