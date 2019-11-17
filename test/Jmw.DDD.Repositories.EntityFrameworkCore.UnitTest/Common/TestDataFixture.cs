// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.

namespace Jmw.DDD.Repositories.EntityFrameworkCoreUnitTest.Common
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Test data fixture.
    /// </summary>
    [Table(nameof(TestDataFixture), Schema = "Schema")]
    public class TestDataFixture
    {
        /// <summary>
        /// Gets or sets gets the data Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets gets a collection of data.
        /// </summary>
        public IList<TestDataChildFixture> Collection { get; set; }

        /// <summary>
        /// Gets or sets gets a data by reference.
        /// </summary>
        public TestDataChildFixture Reference { get; set; }
    }
}
