// <copyright file="ReadOnlyRepositoryFixture.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Jmw.DDD.Repositories.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Fixture of <see cref="ReadOnlyRepository{TContext, TData, TKey}" />
    /// for unit testing.
    /// </summary>
    public class ReadOnlyRepositoryFixture :
        ReadOnlyRepository<DbContextFixture, TestDataFixture, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepositoryFixture"/> class.
        /// </summary>
        /// <param name="includeReferences">Indicates if we must include collections and references when querying entities.</param>
        public ReadOnlyRepositoryFixture(bool includeReferences)
            : base(new DbContextFixture(), p => p.TestData)
        {
            IncludeReferences = includeReferences;

            Seed();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepositoryFixture"/> class.
        /// </summary>
        /// <param name="dbContext">DbContext to use.</param>
        /// <param name="propertySelector">Property selector.</param>
        public ReadOnlyRepositoryFixture(
            DbContextFixture dbContext,
            Func<DbContextFixture, DbSet<TestDataFixture>> propertySelector)
            : base(dbContext, propertySelector)
        {
            Seed();
        }

        /// <summary>
        /// Gets a value indicating whether indicates if we should include references.
        /// </summary>
        public bool IncludeReferences { get; }

        /// <inheritdoc/>
        protected override void OnConfigure(RepositoryConfiguration<DbContextFixture, TestDataFixture> configuration)
        {
            configuration
                .OrderBy(m => m.Id);

            if (IncludeReferences)
            {
                configuration
                    .Include(m => m.Collection)
                    .Include(m => m.Reference);
            }
        }

        /// <summary>
        /// Adds some sample data.
        /// </summary>
        private void Seed()
        {
            var fixture = new Fixture();
            var testData = fixture.Create<IEnumerable<TestDataFixture>>();

            Configuration.Context.AddRange(testData);

            Configuration.Context.SaveChanges();
        }
    }
}
