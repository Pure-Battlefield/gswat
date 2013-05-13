using Microsoft.WindowsAzure.ServiceRuntime;

namespace core.Utilities
{
    /// <summary>
    /// Provides a testable interface for RoleEnvironments settings
    /// </summary>
    public interface ICloudSettingsManager
    {
        /// <summary>
        /// Retrieve a setting value from the underlying Environment.
        /// </summary>
        /// <param name="setting">The name of the setting to retrieve.</param>
        /// <returns>The value specified for the setting given, or throws an exception if setting does not exist.</returns>
        string GetConfigurationSettingValue(string setting);
    }
    public class CloudSettingsManager : ICloudSettingsManager
    {
        public string GetConfigurationSettingValue(string setting)
        {
            return RoleEnvironment.GetConfigurationSettingValue(setting);
        }
    }
}
