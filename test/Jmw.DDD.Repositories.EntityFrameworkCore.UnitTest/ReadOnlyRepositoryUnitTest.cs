// <copyright file="ReadOnlyRepositoryUnitTest.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Jmw.DDD.Domain.Repositories;
    using Jmw.DDD.Repositories.EntityFrameworkCore;
    using Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common;
    using Xunit;

    /// <summary>
    /// <see cref="ReadOnlyRepository{TContext, TData, TKey}"/> unit tests.
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
            Action sut1 = () => new ReadOnlyRepositoryFixture(new DbContextFixture(), null);
            Action sut2 = () => new ReadOnlyRepositoryFixture(null, c => c.TestData);

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(sut1);
            Assert.Throws<ArgumentNullException>(sut2);
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

            // Act
            var sut = new ReadOnlyRepositoryFixture(dbContext, c => c.TestData);

            // Assert
            Assert.NotNull(sut.Configuration);
        }

        /// <summary>
        /// Checks that <see cref="RepositoryBase{TContext, TData}.QueryAsync" />
        /// correctly checks the parameters.
        /// </summary>
        [Fact]
        [Trait("Repositories", "ReadOnlyRepository")]
        public async void QueryAsync_MustCheck_Parameters()
        {
            // Arrange
            var repository = new ReadOnlyRepositoryFixture(false);

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.QueryAsync(null, 0, 100));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.QueryAsync((o) => false, -1, 100));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.QueryAsync((o) => false, 0, -1));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.QueryAsync((o) => false, 0, 1, (SortOrder)(-1)));
        }

        /// <summary>
        /// Checks that <see cref="RepositoryBase{TContext, TData}.AnyAsync" />
        /// correctly checks the parameters.
        /// </summary>
        [Fact]
        [Trait("Repositories", "ReadOnlyRepository")]
        public async void AnyAsync_MustCheck_Parameters()
        {
            // Arrange
            var repository = new ReadOnlyRepositoryFixture(false);

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.AnyAsync(-1, 100));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.AnyAsync(0, -1));
        }

        /// <summary>
        /// Checks that <see cref="RepositoryBase{TContext, TData}.CountAsync"/>
        /// returns the correct Count.
        /// </summary>
        /// <param name="includeReferences">Includes and check references.</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [Trait("Repositories", "ReadOnlyRepository")]
        public async void CountAsync_MustReturn_Count(bool includeReferences)
        {
            // Arrange
            var sut = new ReadOnlyRepositoryFixture(includeReferences);

            // Act
            var computed = await sut.CountAsync();
            var computed2 = await sut.CountAsync(e => false);

            // Assert
            Assert.Equal(sut.Configuration.DbSet.Count(), computed);
            Assert.Equal(0, computed2);
        }

        /// <summary>
        /// Checks that <see cref="ReadOnlyRepository{TContext, TData, TKey}.FindAsync"/>
        /// returns the correct entity.
        /// </summary>
        /// <param name="includeReferences">Includes and check references.</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [Trait("Repositories", "ReadOnlyRepository")]
        public async void FindAsync_MustReturn_Entity(bool includeReferences)
        {
            // Arrange
            var sut = new ReadOnlyRepositoryFixture(includeReferences);
            var entity = sut.Configuration.DbSet.Last();

            // Act
            var computed = await sut.FindAsync(entity.Id);

            // Assert
            Assert.NotNull(computed.Id);
            Assert.Equal(entity.Id, computed.Id);

            if (includeReferences)
            {
                Assert.NotNull(computed.Reference);
                Assert.NotNull(computed.Collection);
                Assert.NotEmpty(computed.Collection);
            }
        }

        /// <summary>
        /// Checks that <see cref="RepositoryBase{TContext, TData}.FirstAsync"/>
        /// returns the correct entity.
        /// </summary>
        /// <param name="includeReferences">Includes and check references.</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [Trait("Repositories", "ReadOnlyRepository")]
        public async void FirstAsync_MustReturn_Entity(bool includeReferences)
        {
            // Arrange
            var sut = new ReadOnlyRepositoryFixture(includeReferences);
            var firstEntity = sut.Configuration.DbSet.OrderBy(e => e.Id).First();

            // Act
            var computed = await sut.FirstAsync();
            var computed2 = await sut.FirstAsync(e => e.Id != firstEntity.Id);

            // Assert
            Assert.Equal(firstEntity.Id, computed.Id);
            Assert.NotNull(computed2);
            Assert.NotEqual(firstEntity.Id, computed2.Id);

            if (includeReferences)
            {
                Assert.NotNull(computed.Reference);
                Assert.NotNull(computed.Collection);
                Assert.NotEmpty(computed.Collection);
            }
        }

        /// <summary>
        /// Checks that <see cref="RepositoryBase{TContext, TData}.LastAsync"/>
        /// returns the correct entity.
        /// </summary>
        /// <param name="includeReferences">Includes and check references.</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [Trait("Repositories", "ReadOnlyRepository")]
        public async void LastAsync_MustReturn_Entity(bool includeReferences)
        {
            // Arrange
            var sut = new ReadOnlyRepositoryFixture(includeReferences);
            var lastEntity = sut.Configuration.DbSet.OrderBy(e => e.Id).Last();

            // Act
            var computed = await sut.LastAsync();
            var computed2 = await sut.LastAsync(e => e.Id != lastEntity.Id);

            // Assert
            Assert.Equal(lastEntity.Id, computed.Id);
            Assert.NotNull(computed2);
            Assert.NotEqual(lastEntity.Id, computed2.Id);
            if (includeReferences)
            {
                Assert.NotNull(computed.Reference);
                Assert.NotNull(computed.Collection);
                Assert.NotEmpty(computed.Collection);
            }
        }

        /// <summary>
        /// Checks that <see cref="RepositoryBase{TContext, TData}.AnyAsync"/>
        /// returns the correct entities.
        /// </summary>
        /// <param name="includeReferences">Includes and check references.</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [Trait("Repositories", "ReadOnlyRepository")]
        public async void AnyAsync_MustReturn_Entity(bool includeReferences)
        {
            // Arrange
            var sut = new ReadOnlyRepositoryFixture(includeReferences);
            var enumerator = sut.Configuration.DbSet.OrderBy(e => e.Id).GetEnumerator();
            var last = sut.Configuration.DbSet.OrderByDescending(e => e.Id).First();

            // Act
            var computed = await sut.AnyAsync(0, int.MaxValue);

            // Assert
            Assert.Equal(sut.Configuration.DbSet.Count(), computed.Count());
            Assert.All(computed, c =>
            {
                enumerator.MoveNext();
                Assert.Equal(enumerator.Current.Id, c.Id);
                if (includeReferences)
                {
                    Assert.NotNull(enumerator.Current.Reference);
                    Assert.NotNull(enumerator.Current.Collection);
                    Assert.NotEmpty(enumerator.Current.Collection);
                }
            });
        }

        /// <summary>
        /// Checks that <see cref="RepositoryBase{TContext, TData}.QueryAsync"/>
        /// returns the correct entities.
        /// </summary>
        /// <param name="includeReferences">Includes and check references.</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [Trait("Repositories", "ReadOnlyRepository")]
        public async void QueryAsync_MustReturn_Entity(bool includeReferences)
        {
            // Arrange
            var sut = new ReadOnlyRepositoryFixture(includeReferences);
            var entite = sut.Configuration.DbSet.First();
            var entities = sut.Configuration.DbSet.Where(e => e.Id != entite.Id).OrderBy(e => e.Id);
            var enumerator = entities.GetEnumerator();

            // Act
            var computed = await sut.QueryAsync(e => e.Id != entite.Id, 0, int.MaxValue);

            // Assert
            Assert.Equal(entities.Count(), computed.Count());
            Assert.All(computed, c =>
            {
                enumerator.MoveNext();
                Assert.Equal(enumerator.Current.Id, c.Id);
                if (includeReferences)
                {
                    Assert.NotNull(enumerator.Current.Reference);
                    Assert.NotNull(enumerator.Current.Collection);
                    Assert.NotEmpty(enumerator.Current.Collection);
                }
            });
        }
    }
}
