// <copyright file="IniConfigurationExtensions.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.DDD.Configuration
{
    using Dawn;
    using Jmw.DDD.Domain.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extensions for configuration from ini files.
    /// </summary>
    public static class IniConfigurationExtensions
    {
        /// <summary>
        /// Adds the INI configuration provider at path to builder.
        /// </summary>
        /// <param name="builder">The Microsoft.Extensions.Configuration.IConfigurationBuilder to add to.</param>
        /// <param name="appName">Application Name.</param>
        /// <param name="company">Optional company name.</param>
        /// <param name="fileName">Optional filename.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <returns>The Microsoft.Extensions.Configuration.IConfigurationBuilder.</returns>
        public static IConfigurationBuilder AddCommonApplicationDataIniFile(
            this IConfigurationBuilder builder,
            string appName,
            string company = "",
            string fileName = "machineSettings.ini",
            bool optional = false,
            bool reloadOnChange = false)
        {
            Guard.Argument(builder, nameof(builder)).NotNull();
            Guard.Argument(appName, nameof(appName)).NotNull().NotEmpty();
            Guard.Argument(fileName, nameof(fileName)).NotNull().NotEmpty();

            var ini = MakeFileName(appName, company, fileName);

            if (!optional && !File.Exists(ini))
            {
                using var f = File.Create(ini);
            }

            builder.AddIniFile(ini, optional, reloadOnChange);

            return builder;
        }

        /// <summary>
        /// Adds an <see cref="IOptionWriter"/> that writes Options to an INI file into service collection.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection instance.</param>
        /// <param name="appName">Application Name.</param>
        /// <param name="company">Optional company name.</param>
        /// <param name="fileName">Optional filename.</param>
        /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection.</returns>
        public static IServiceCollection AddCommonApplicationDataIniWriter(
            this IServiceCollection services,
            string appName,
            string company = "",
            string fileName = "machineSettings.ini")
        {
            Guard.Argument(services, nameof(services)).NotNull();
            Guard.Argument(appName, nameof(appName)).NotNull().NotEmpty();
            Guard.Argument(fileName, nameof(fileName)).NotNull().NotEmpty();

            var ini = MakeFileName(appName, company, fileName);

            services.AddTransient<IOptionWriter>(f => new IniOptionWriter(ini));

            return services;
        }

        private static string MakeFileName(
            string appName,
            string company,
            string fileName)
        {
            if (string.IsNullOrEmpty(company))
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    appName,
                    fileName);
            }
            else
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    company,
                    appName,
                    fileName);
            }
        }
    }
}