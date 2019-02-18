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
    /// Fixture of <see cref="ReadOnlyRepository{TContext, TData, TKey, TOrderBy}" />
    /// for unit testing.
    /// </summary>
    public class ReadOnlyRepositoryFixture :
        ReadOnlyRepository<DbContextFixture, TestDataFixture, string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepositoryFixture"/> class.
        /// </summary>
        /// <param name="includeReferences">Indicates if we must include collections and references when querying entities.</param>
        public ReadOnlyRepositoryFixture(bool includeReferences)
            : base(new DbContextFixture(), p => p.TestData)
        {
            SetOrderBy(m => m.Id);

            if (includeReferences)
            {
                SetIncludes(m => m.Collection, m => m.Reference);
            }

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
            SetOrderBy(m => m.Id);

            SetIncludes(m => m.Collection, m => m.Reference);
        }

        /// <summary>
        /// Adds some sample data.
        /// </summary>
        private void Seed()
        {
            var fixture = new Fixture();
            var testData = fixture.Create<IEnumerable<TestDataFixture>>();

            this.Context.AddRange(testData);

            this.Context.SaveChanges();
        }
    }
}
