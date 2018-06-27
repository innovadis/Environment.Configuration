using System;
using System.Configuration;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Environment.Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Returns an IConfigurationSection that contains the values from the environment variables
        /// as described by the ConfigurationPropertyAttribute per property
        /// </summary>
        /// <param name="configuration"></param>
        /// <typeparam name="TSection"></typeparam>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public static TSection GetEnvironmentSection<TSection>(this IConfiguration configuration)
            where TSection : class, IConfigurationSection
        {
            // Get the type of generic TSection which is an implementation of IConfigurationSection
            var type = typeof(TSection);

            // Get the properties from the type
            var properties = type.GetProperties();
            
            // Create a new instance of our generic and insert the configuration as a parameter
            var configurationSection = (TSection) Activator.CreateInstance(type, configuration);

            foreach (var property in properties)
            {
                // If the property is not decorated with a ConfigurationPropertyAttribute, continue
                if (!(property
                        .GetCustomAttributes(true)
                        .FirstOrDefault(attr => attr is ConfigurationPropertyAttribute) 
                    is ConfigurationPropertyAttribute
                    attribute)) continue;

                // Retrieve the value from the property
                var value = configuration.GetValue<string>(attribute.Name);
                
                // Throw an exception if a setting is marked as required and is null.
                if (value == null && attribute.IsRequired) 
                    throw new ConfigurationErrorsException(
                        $"Environment variable {attribute.Name} is required but is empty or not set");

                configurationSection[attribute.Name] = value;
            }

            return configurationSection;
        }
    }
}