using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.ComponentModel;

namespace Automatic_Alarm
{
    public class Settings
    {
        // Our settings
        IsolatedStorageSettings settings;

        // The key names of our settings
        const string DefaultTimeAutoKey = "TimeSetting";
        const string DefaultTuneKey = "TuneSetting";
        const string DefaultLateTimeKey = "LateTimeSetting";

        // The default value of our settings
        readonly DateTime DefaultTimeAuto = new DateTime().AddHours(1).AddMinutes(41);
        readonly DateTime DefaultLateTime = new DateTime().AddHours(10);
        const string DefaultTune = "Good Morning";
        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public Settings()
        {
            // Get the settings for this application.
            settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains(DefaultTimeAutoKey))
            {
                DefaultTimeAutoSetting = DefaultTimeAuto;
            }
            if (!settings.Contains(DefaultTuneKey))
            {
                DefaultTuneSetting = DefaultTune;
            }
            if (!settings.Contains(DefaultLateTimeKey))
            {
                DefaultLateTimeSetting = DefaultLateTime;
            }
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
           return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }


        /// <summary>
        /// Property to get and set a CheckBox Setting Key.
        /// </summary>
        public DateTime DefaultTimeAutoSetting
        {
            get
            {
                return GetValueOrDefault<DateTime>(DefaultTimeAutoKey, DefaultTimeAuto);
            }
            set
            {
                if (AddOrUpdateValue(DefaultTimeAutoKey, value))
                {
                    Save();
                }
            }
        }
        public string DefaultTuneSetting
        {
            get
            {
                return GetValueOrDefault<string>(DefaultTuneKey, DefaultTune);
            }
            set
            {
                if (AddOrUpdateValue(DefaultTuneKey, value))
                {
                    Save();
                }
            }
        }
        public DateTime DefaultLateTimeSetting
        {
            get
            {
                return GetValueOrDefault<DateTime>(DefaultLateTimeKey, DefaultLateTime);
            }
            set
            {
                if (AddOrUpdateValue(DefaultLateTimeKey, value))
                {
                    Save();
                }
            }
        }
    }
}