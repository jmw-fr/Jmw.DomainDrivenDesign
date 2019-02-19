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
            SetOrderBy(m => m.Id);
            SetIncludes(m => m.Collection, m => m.Reference);
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
            SetOrderBy(m => m.Id);
            SetIncludes(m => m.Collection, m => m.Reference);
        }
    }
}
