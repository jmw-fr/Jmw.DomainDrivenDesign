// <copyright file="TransactionalRepositoryUnitTest.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest
{
    using System;
    using Jmw.DDD.Repositories.EntityFrameworkCore;
    using Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common;
    using Xunit;

    /// <summary>
    /// <see cref="TransactionalRepository{TContext, TData, TKey, TOrderBy}"/> unit tests.
    /// </summary>
    public class TransactionalRepositoryUnitTest
    {
        /// <summary>
        /// Checks that the constructor correctly throws the exceptions.
        /// </summary>
        [Fact]
        [Trait("Repositories", "TransactionalRepository")]
        public void Constructor_Must_ThrowExceptions()
        {
            // Arrange
            Action sut1 = () => new TransactionalRepositoryFixture(new DbContextFixture(), null);
            Action sut2 = () => new TransactionalRepositoryFixture(null, c => c.TestData);

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(sut1);
            Assert.Throws<ArgumentNullException>(sut2);
        }
    }
}
