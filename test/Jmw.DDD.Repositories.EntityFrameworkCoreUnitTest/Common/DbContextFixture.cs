// <copyright file="DbContextFixture.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common
{
    using System;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// A <see cref="DbContext"/> fixture for unit test.
    /// </summary>
    public class DbContextFixture : DbContext
    {
        /// <summary>
        /// Gets or sets the test data DbSet.
        /// </summary>
        public DbSet<TestDataFixture> TestData { get; set; }

        /// <summary>
        /// Gets the database name.
        /// </summary>
        public string DatabaseName { get; } = Guid.NewGuid().ToString();

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        }
    }
}
