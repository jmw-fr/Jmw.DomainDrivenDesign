// <copyright file="RepositoryFixture.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Jmw.DDD.Repositories.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Fixture of <see cref="Repository{TContext, TData, TKey, TOrderBy}" />
    /// for unit testing.
    /// </summary>
    public class RepositoryFixture :
        Repository<DbContextFixture, TestDataFixture, string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFixture"/> class.
        /// </summary>
        public RepositoryFixture()
            : base(new DbContextFixture(), p => p.TestData, o => o.Id, p => p.Collection, p => p.Reference)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFixture"/> class.
        /// </summary>
        /// <param name="dbContext">DbContext to use.</param>
        /// <param name="propertySelector">Property selector.</param>
        /// <param name="orderBySelector">order by property selector</param>
        /// <param name="includes">Includes to add.</param>
        public RepositoryFixture(
            DbContextFixture dbContext,
            Func<DbContextFixture, DbSet<TestDataFixture>> propertySelector,
            Expression<Func<TestDataFixture, string>> orderBySelector,
            params Expression<Func<TestDataFixture, object>>[] includes)
            : base(dbContext, propertySelector, orderBySelector, includes)
        {
        }
    }
}
