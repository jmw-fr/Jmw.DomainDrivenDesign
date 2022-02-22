// <copyright file="IniOptionWriter.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Configuration
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Dawn;
    using Jmw.DDD.Domain.Configuration;

    /// <summary>
    /// Implementation of <see cref="IOptionWriter"/> to an INI file.
    /// </summary>
    public class IniOptionWriter : IOptionWriter
    {
        private readonly string iniCompleteFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="IniOptionWriter"/> class.
        /// </summary>
        /// <param name="iniCompleteFileName">Ini complete filename path.</param>
        internal IniOptionWriter(string iniCompleteFileName)
        {
            this.iniCompleteFileName = Guard.Argument(iniCompleteFileName, nameof(iniCompleteFileName)).NotNull().NotEmpty();
        }

        /// <inheritdoc/>
        public Task WriteOptionAsync<TOption, TValue>(Expression<Func<TOption, TValue>> propertySelector, TValue newValue)
            where TOption : class, IOptionSetting
        {
            Guard.Argument(propertySelector.Body.NodeType, nameof(propertySelector)).Equal(ExpressionType.MemberAccess);

            string propertyName = ((MemberExpression)propertySelector.Body).Member.Name;

            var p = typeof(TOption)
                .GetField(
                    nameof(IOptionSetting.SectionName),
                    BindingFlags.Static | BindingFlags.Public)
                ?? throw new NullReferenceException();

            var section = p.GetValue(null) as string ?? string.Empty;

            var f = new PeanutButter.INI.INIFile(this.iniCompleteFileName);

            if (File.Exists(this.iniCompleteFileName))
            {
                f.Reload();
            }

            f.SetValue(section, propertyName, newValue?.ToString());

            f.Persist();

            return Task.CompletedTask;
        }
    }
}
