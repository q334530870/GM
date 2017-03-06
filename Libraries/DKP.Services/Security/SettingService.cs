using DKP.Core.Caching;
using DKP.Core.Configuration;
using DKP.Core.Data;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DKP.Services.Security
{
    /// <summary>
    /// Setting service interface
    /// </summary>
    public partial interface ISettingService
    {
        T LoadSetting<T>() where T : ISettings, new();
        T GetSettingByKey<T>(string key, T defaultValue = default(T), bool loadSharedValueIfNotFound = false);
    }

    /// <summary>
    /// Setting manager
    /// </summary>
    public partial class SettingService : ISettingService
    {
        private const string SETTINGS_ALL_KEY = "DKP.setting.all";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string SETTINGS_PATTERN_KEY = "DKP.setting.";
        private readonly ICacheManager _cacheManager;

        public SettingService(ICacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public virtual T LoadSetting<T>() where T : ISettings, new()
        {
            var settings = Activator.CreateInstance<T>();

            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                //load by store
                var setting = GetSettingByKey<string>(key, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!CommonHelper.GetPapayaCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!CommonHelper.GetPapayaCustomTypeConverter(prop.PropertyType).IsValid(setting))
                    continue;

                object value = CommonHelper.GetPapayaCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings;
        }
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T),
            bool loadSharedValueIfNotFound = false)
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;
            return defaultValue;
        }
    }
}
