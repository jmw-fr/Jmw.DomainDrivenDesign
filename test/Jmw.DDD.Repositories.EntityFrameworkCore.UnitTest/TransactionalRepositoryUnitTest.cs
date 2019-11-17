// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Jmw.DDD.Application;
    using Jmw.DDD.Repositories.EntityFrameworkCore;
    using Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common;
    using Xunit;

    /// <summary>
    /// <see cref="TransactionalRepository{TContext, TData, TKey}"/> unit tests.
    /// </summary>
    [Trait("Repositories", "ReadWriteRepository")]
    public class TransactionalRepositoryUnitTest
    {
        /// <summary>
        /// Checks that the constructor correctly throws the exceptions.
        /// </summary>
        [Fact]
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

        /// <summary>
        /// Checks that the constructor correctly assign properties.
        /// </summary>
        [Fact]
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

        /// <summary>
        /// Checks that <see cref="TransactionalRepository{TContext, TData, TKey}.QueryAsync" />
        /// correctly checks the parameters.
        /// </summary>
        [Fact]
        [Trait("Repositories", "ReadOnlyRepository")]
        public async void QueryAsync_MustCheck_Parameters()
        {
            // Arrange
            var repository = new TransactionalRepositoryFixture();

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.QueryAsync(null, 0, 100));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.QueryAsync((o) => false, -1, 100));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.QueryAsync((o) => false, 0, -1));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.QueryAsync((o) => false, 0, 1, (SortOrder)(-1)));
        }

        /// <summary>
        /// Checks that <see cref="TransactionalRepository{TContext, TData, TKey}.AnyAsync" />
        /// correctly checks the parameters.
        /// </summary>
        [Fact]
        public async void AnyAsync_MustCheck_Parameters()
        {
            // Arrange
            var repository = new TransactionalRepositoryFixture();

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.AnyAsync(-1, 100));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await repository.AnyAsync(0, -1));
        }

        /// <summary>
        /// Checks that <see cref="TransactionalRepository{TContext, TData, TKey}.CountAsync"/>
        /// returns the correct Count.
        /// </summary>
        [Fact]
        public async void CountAsync_MustReturn_Count()
        {
            // Arrange
            var sut = new TransactionalRepositoryFixture();

            // Act
            var computed = await sut.CountAsync();
            var computed2 = await sut.CountAsync(e => false);

            // Assert
            Assert.Equal(sut.Configuration.DbSet.Count(), computed);
            Assert.Equal(0, computed2);
        }

        /// <summary>
        /// Checks that <see cref="TransactionalRepository{TContext, TData, TKey}.FindAsync"/>
        /// returns the correct entity.
        /// </summary>
        [Fact]
        public async void FindAsync_MustReturn_Entity()
        {
            // Arrange
            var sut = new TransactionalRepositoryFixture();
            var entity = sut.Configuration.DbSet.Last();

            // Act
            var computed = await sut.FindAsync(entity.Id);

            // Assert
            Assert.NotNull(computed);
            Assert.Equal(entity, computed);
        }

        /// <summary>
        /// Checks that <see cref="TransactionalRepository{TContext, TData, TKey}.FirstAsync"/>
        /// returns the correct entity.
        /// </summary>
        [Fact]
        public async void FirstAsync_MustReturn_Entity()
        {
            // Arrange
            var sut = new TransactionalRepositoryFixture();
            var firstEntity = sut.Configuration.DbSet.OrderBy(e => e.Id).First();

            // Act
            var computed = await sut.FirstAsync();
            var computed2 = await sut.FirstAsync(e => e.Id != firstEntity.Id);

            // Assert
            Assert.NotNull(computed);
            Assert.NotNull(computed2);
            Assert.Equal(firstEntity, computed2);
            Assert.NotEqual(firstEntity.Id, computed2.Id);
        }

        /// <summary>
        /// Checks that <see cref="TransactionalRepository{TContext, TData, TKey}.LastAsync"/>
        /// returns the correct entity.
        /// </summary>
        [Fact]
        public async void LastAsync_MustReturn_Entity()
        {
            // Arrange
            var sut = new TransactionalRepositoryFixture();
            var lastEntity = sut.Configuration.DbSet.OrderBy(e => e.Id).Last();

            // Act
            var computed = await sut.LastAsync();
            var computed2 = await sut.LastAsync(e => e.Id != lastEntity.Id);

            // Assert
            Assert.Equal(lastEntity.Id, computed.Id);
            Assert.NotNull(computed2);
            Assert.NotEqual(lastEntity.Id, computed2.Id);
            Assert.NotNull(computed.Reference);
            Assert.NotNull(computed.Collection);
            Assert.NotEmpty(computed.Collection);
        }

        /// <summary>
        /// Checks that <see cref="TransactionalRepository{TContext, TData, TKey}.AnyAsync"/>
        /// returns the correct entities.
        /// </summary>
        [Fact]
        public async void AnyAsync_MustReturn_Entity()
        {
            // Arrange
            var sut = new TransactionalRepositoryFixture();
            var enumerator = sut.Configuration.DbSet.OrderBy(e => e.Id).GetEnumerator();
            var last = sut.Configuration.DbSet.OrderByDescending(e => e.Id).First();

            // Act
            var computed = await sut.AnyAsync(0, int.MaxValue);

            // Assert
            Assert.Equal(sut.Configuration.DbSet.Count(), computed.Count());
        }

        /// <summary>
        /// Checks that <see cref="TransactionalRepository{TContext, TData, TKey}.QueryAsync"/>
        /// returns the correct entities.
        /// </summary>
        [Fact]
        public async void QueryAsync_MustReturn_Entity()
        {
            // Arrange
            var sut = new TransactionalRepositoryFixture();
            var entite = sut.Configuration.DbSet.First();
            var entities = sut.Configuration.DbSet.Where(e => e.Id != entite.Id).OrderBy(e => e.Id);
            var enumerator = entities.GetEnumerator();

            // Act
            var computed = await sut.QueryAsync(e => e.Id != entite.Id, 0, int.MaxValue);

            // Assert
            Assert.Equal(entities.Count(), computed.Count());
        }

        /// <summary>
        /// Checks that <see cref="TransactionalRepository{TContext, TData, TKey}.InsertAsync" />
        /// correctly checks the parameters.
        /// </summary>
        [Fact]
        public async void InsertAsync_MustCheck_Parameters()
        {
            // Arrange
            var repository = new TransactionalRepositoryFixture();

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.InsertAsync(null));
        }

        /// <summary>
        /// Checks that <see cref="TransactionalRepository{TContext, TData, TKey}.UpdateAsync" />
        /// correctly checks the parameters.
        /// </summary>
        [Fact]
        public async void UpdateAsync_MustCheck_Parameters()
        {
            // Arrange
            var repository = new TransactionalRepositoryFixture();

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.UpdateAsync(null));
        }
    }
}
