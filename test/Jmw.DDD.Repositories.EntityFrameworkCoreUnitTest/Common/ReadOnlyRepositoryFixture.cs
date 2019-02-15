// <copyright file="ReadOnlyRepositoryFixture.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
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
        /// <param name="includes">Indicates if we must include collections and references when querying entities.</param>
        public ReadOnlyRepositoryFixture(bool includes = false)
            : base(new DbContextFixture(), p => p.TestData, o => o.Id, p => p.Collection, p => p.Reference)
        {
            Seed();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepositoryFixture"/> class.
        /// </summary>
        /// <param name="dbContext">DbContext to use.</param>
        /// <param name="propertySelector">Property selector.</param>
        /// <param name="orderBySelector">order by property selector</param>
        /// <param name="includes">Includes to add.</param>
        public ReadOnlyRepositoryFixture(
            DbContextFixture dbContext,
            Func<DbContextFixture, DbSet<TestDataFixture>> propertySelector,
            Expression<Func<TestDataFixture, string>> orderBySelector,
            params Expression<Func<TestDataFixture, object>>[] includes)
            : base(dbContext, propertySelector, orderBySelector, includes)
        {
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
