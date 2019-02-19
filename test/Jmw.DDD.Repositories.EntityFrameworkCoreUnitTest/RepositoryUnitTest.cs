// <copyright file="RepositoryUnitTest.cs" company="Jean-Marc Weeger">
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
    /// <see cref="Repository{TContext, TData, TKey}"/> unit tests.
    /// </summary>
    public class RepositoryUnitTest
    {
        /// <summary>
        /// Checks that the constructor correctly throws the exceptions.
        /// </summary>
        [Fact]
        [Trait("Repositories", "Repository")]
        public void Constructor_Must_ThrowExceptions()
        {
            // Arrange
            Action sut1 = () => new RepositoryFixture(new DbContextFixture(), null);
            Action sut2 = () => new RepositoryFixture(null, c => c.TestData);
            Action sut3 = () => new RepositoryFixture(new DbContextFixture(), c => null);

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
        [Trait("Repositories", "Repository")]
        public void Constructor_MustSet_Properties()
        {
            // Arrange
            var fixture = new Fixture();
            var dbContext = new DbContextFixture();

            // Act
            var sut = new RepositoryFixture(dbContext, c => c.TestData);

            // Assert
            Assert.Equal(dbContext, sut.Context);
            Assert.Equal(dbContext.TestData, sut.DbSet);
        }

        /// <summary>
        /// Checks that <see cref="Repository{TContext, TData, TKey}.InsertAsync" />
        /// correctly checks the parameters.
        /// </summary>
        [Fact]
        [Trait("Repositories", "Repository")]
        public async void InsertAsync_MustCheck_Parameters()
        {
            // Arrange
            var repository = new RepositoryFixture();

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.InsertAsync(null));
        }

        /// <summary>
        /// Checks that <see cref="Repository{TContext, TData, TKey}.UpdateAsync" />
        /// correctly checks the parameters.
        /// </summary>
        [Fact]
        [Trait("Repositories", "Repository")]
        public async void UpdateAsync_MustCheck_Parameters()
        {
            // Arrange
            var repository = new RepositoryFixture();

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.UpdateAsync(null));
        }
    }
}
