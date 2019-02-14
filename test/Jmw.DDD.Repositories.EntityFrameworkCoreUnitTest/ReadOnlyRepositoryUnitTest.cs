// <copyright file="ReadOnlyRepositoryUnitTest.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using AutoFixture;
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
            Action sut1 = () => new ReadOnlyRepositoryFixture(new DbContextFixture(), null, null, null);
            Action sut2 = () => new ReadOnlyRepositoryFixture(null, c => c.TestData, null, null);
            Action sut3 = () => new ReadOnlyRepositoryFixture(new DbContextFixture(), c => null, null, null);

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
        [Trait("Repositories", "ReadOnlyRepository")]
        public void Constructor_MustSet_Properties()
        {
            // Arrange
            var fixture = new Fixture();
            var dbContext = new DbContextFixture();
            Expression<Func<TestDataFixture, string>> orderBySelector = o => o.Id;
            var includes = fixture.Create<IEnumerable<string>>();

            // Act
            var sut = new ReadOnlyRepositoryFixture(dbContext, c => c.TestData, orderBySelector, includes);

            // Assert
            Assert.Equal(dbContext, sut.Context);
            Assert.Equal(dbContext.TestData, sut.DbSet);
            Assert.Equal(orderBySelector, sut.OrderBySelector);
            Assert.Equal(includes, sut.Includes);
        }

        /// <summary>
        /// Checks that <see cref="ReadOnlyRepository{TContext, TData, TKey, TOrderBy}.QueryAsync" />
        /// correctly checks the parameters.
        /// </summary>
        [Fact]
        [Trait("Repositories", "ReadOnlyRepository")]
        public async void QueryAsync_MustCheck_Parameters()
        {
            // Arrange
            var repository = new ReadOnlyRepositoryFixture();

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.QueryAsync(null, 0, 100, false));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.QueryAsync((o) => false, -1, 100, false));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.QueryAsync((o) => false, 0, -1, false));
        }

        /// <summary>
        /// Checks that <see cref="ReadOnlyRepository{TContext, TData, TKey, TOrderBy}.AnyAsync" />
        /// correctly checks the parameters.
        /// </summary>
        [Fact]
        [Trait("Repositories", "ReadOnlyRepository")]
        public async void AnyAsync_MustCheck_Parameters()
        {
            // Arrange
            var repository = new ReadOnlyRepositoryFixture();

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.AnyAsync(-1, 100, false));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.AnyAsync(0, -1, false));
        }
    }
}
