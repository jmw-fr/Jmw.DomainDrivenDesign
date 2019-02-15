// <copyright file="TestDataFixture.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Test data fixture.
    /// </summary>
    [Table(nameof(TestDataFixture), Schema = "Schema")]
    public class TestDataFixture
    {
        /// <summary>
        /// Gets the data Id.
        /// </summary>
        public string Id { get; internal set; }
    }
}
