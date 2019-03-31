using System;
using System.Collections.Generic;
using System.Text;

namespace Vector.Explorer.ViewModel
{
	public interface ISettingsService
	{
		void Clear();
		void Set<T>(string key, T value);
		T Get<T>(string key, T defaultValue = default(T));
		void Remove(string key);
	}
}
