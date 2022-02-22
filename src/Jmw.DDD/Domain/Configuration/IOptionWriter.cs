// <copyright file="IOptionWriter.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Domain.Configuration
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a write of settings.
    /// </summary>
    public interface IOptionWriter
    {
        /// <summary>
        /// Writes an option value.
        /// </summary>
        /// <param name="propertySelector">Selector of the property from TOption.</param>
        /// <param name="newValue">New Value to write.</param>
        /// <typeparam name="TOption">Option type.</typeparam>
        /// <typeparam name="TValue">Type of the value to write.</typeparam>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task WriteOptionAsync<TOption, TValue>(Expression<Func<TOption, TValue>> propertySelector, TValue newValue)
            where TOption : class, IOptionSetting;
    }
}
