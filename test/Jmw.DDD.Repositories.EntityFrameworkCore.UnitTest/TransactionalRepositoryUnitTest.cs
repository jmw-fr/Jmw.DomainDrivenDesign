﻿// <copyright file="TransactionalRepositoryUnitTest.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest
{
    using System;
    using AutoFixture;
    using Jmw.DDD.Repositories.EntityFrameworkCore;
    using Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common;
    using Xunit;

    /// <summary>
    /// <see cref="TransactionalRepository{TContext, TData, TKey}"/> unit tests.
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
            Action sut3 = () => new TransactionalRepositoryFixture(new DbContextFixture(), c => null);

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(sut1);
            Assert.Throws<ArgumentNullException>(sut2);
            Assert.Throws<InvalidOperationException>(sut3);
        }

        /// <summary>
        /// Checks that the constructor correctly assign properties.
        /// </summary>
        [Fact]
        [Trait("Repositories", "TransactionalRepository")]
        public void Constructor_MustSet_Properties()
        {
            // Arrange
            var fixture = new Fixture();
            var dbContext = new DbContextFixture();

            // Act
            var sut = new TransactionalRepositoryFixture(dbContext, c => c.TestData);

            // Assert
            Assert.NotNull(sut.Configuration);
        }
    }
}
