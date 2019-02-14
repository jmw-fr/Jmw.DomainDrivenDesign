// <copyright file="TransactionalRepositoryFixture.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common
{
    using System;
    using Jmw.DDD.Repositories.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Fixture of <see cref="TransactionalRepository{TContext, TData, TKey, TOrderBy}" />
    /// for unit testing.
    /// </summary>
    public class TransactionalRepositoryFixture :
        TransactionalRepository<DbContextFixture, TestDataFixture, string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalRepositoryFixture"/> class.
        /// </summary>
        /// <param name="dbContext">DbContext to use.</param>
        /// <param name="propertySelector">Property selector.</param>
        public TransactionalRepositoryFixture(
            DbContextFixture dbContext,
            Func<DbContextFixture, DbSet<TestDataFixture>> propertySelector)
            : base(dbContext, propertySelector)
        {
        }
    }
}
