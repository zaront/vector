using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Newtonsoft.Json;

namespace Vector.Explorer.ViewModel
{
	public class SettingsService : ISettingsService
	{
		public T Get<T>(string key, T defaultValue = default(T))
		{
			switch (defaultValue)
			{
				case bool boolValue:
					return (T)(object)Preferences.Get(key, boolValue);
				case DateTime dateValue:
					return (T)(object)Preferences.Get(key, dateValue);
				case double doubleValue:
					return (T)(object)Preferences.Get(key, doubleValue);
				case float floatValue:
					return (T)(object)Preferences.Get(key, floatValue);
				case int intValue:
					return (T)(object)Preferences.Get(key, intValue);
				case long longValue:
					return (T)(object)Preferences.Get(key, longValue);
				case string stringValue:
					return (T)(object)Preferences.Get(key, stringValue);
				default:
					//deserialize
					return JsonConvert.DeserializeObject<T>(Preferences.Get(key, string.Empty));
			}
		}

		public void Set<T>(string key, T value)
		{
			switch (value)
			{
				case bool boolValue:
					Preferences.Set(key, boolValue);
					break;
				case DateTime dateValue:
					Preferences.Set(key, dateValue);
					break;
				case double doubleValue:
					Preferences.Set(key, doubleValue);
					break;
				case float floatValue:
					Preferences.Set(key, floatValue);
					break;
				case int intValue:
					Preferences.Set(key, intValue);
					break;
				case long longValue:
					Preferences.Set(key, longValue);
					break;
				case string stringValue:
					Preferences.Set(key, stringValue);
					break;
				default:
					//serialize
					Preferences.Set(key, JsonConvert.SerializeObject(value));
					break;
			}
		}

		public void Remove(string key)
		{
			Preferences.Remove(key);
		}

		public void Clear()
		{
			Preferences.Clear();
		}
	}
}
