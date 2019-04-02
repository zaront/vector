using System;
using System.Collections.Generic;
using System.Text;

namespace Vector.Explorer.ViewModel
{
	public class ConnectionStorage : IRobotConnectionInfoStorage
	{
		ISettingsService _settings;

		public ConnectionStorage(ISettingsService settings)
		{
			//set fields
			_settings = settings;
		}

		public RobotConnectionInfo Get(string robotName)
		{
			return _settings.Get<RobotConnectionInfo>(GetKey(robotName));
		}

		public void Remove(string robotName)
		{
			_settings.Remove(GetKey(robotName));
		}

		public void Save(RobotConnectionInfo connection)
		{
			_settings.Set(GetKey(connection.RobotName), connection);
		}

		string GetKey(string robotName)
		{
			return $"{robotName}_RobotConnectionInfo";
		}
	}
}
