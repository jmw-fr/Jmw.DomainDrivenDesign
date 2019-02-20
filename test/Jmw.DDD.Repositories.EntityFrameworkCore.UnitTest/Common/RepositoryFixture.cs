// <copyright file="RepositoryFixture.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common
{
    using System;
    using Jmw.DDD.Repositories.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Fixture of <see cref="Repository{TContext, TData, TKey}" />
    /// for unit testing.
    /// </summary>
    public class RepositoryFixture :
        Repository<DbContextFixture, TestDataFixture, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFixture"/> class.
        /// </summary>
        public RepositoryFixture()
            : base(new DbContextFixture(), p => p.TestData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFixture"/> class.
        /// </summary>
        /// <param name="dbContext">DbContext to use.</param>
        /// <param name="propertySelector">Property selector.</param>
        public RepositoryFixture(
            DbContextFixture dbContext,
            Func<DbContextFixture, DbSet<TestDataFixture>> propertySelector)
            : base(dbContext, propertySelector)
        {
        }

        /// <inheritdoc/>
        protected override void OnConfigure(RepositoryConfiguration<DbContextFixture, TestDataFixture> configuration)
        {
            configuration
                .OrderBy(m => m.Id);

            configuration
                .Include(m => m.Collection)
                .Include(m => m.Reference);
        }
    }
}
