using System;

namespace SchoolUtils
{
    public class AppSettings : IAppSettings
    {
        public T Get<T>(string name, T defaultValue = default)
        {
            string value = Environment.GetEnvironmentVariable(name);
            if (value == null)
            {
                return defaultValue;
            }

            if (typeof(T) == typeof(int))
            {
                return (T)(object)Convert.ToInt32(value);
            }

            if (typeof(T) == typeof(double))
            {
                return (T)(object)Convert.ToDouble(value);
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)value;
            }

            throw new Exception("Unknown type!" + typeof(T));
        }
    }
}
