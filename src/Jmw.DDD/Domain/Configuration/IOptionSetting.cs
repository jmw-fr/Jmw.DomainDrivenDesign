// <copyright file="IOptionSetting.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Domain.Configuration
{
    /// <summary>
    /// Represents an setting option.
    /// </summary>
    public interface IOptionSetting
    {
        /// <summary>
        /// Gets the name of the section in the settings files.
        /// </summary>
        static string SectionName { get; }
    }
}
