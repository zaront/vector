using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Vector
{
	public class RobotConnectionInfoStorage : IRobotConnectionInfoStorage
	{
		public RobotConnectionInfo Get(string robotName)
		{
			var filePath = GetFilePath(robotName);
			if (File.Exists(filePath))
				return JsonConvert.DeserializeObject<RobotConnectionInfo>(File.ReadAllText(filePath));
			return null;
		}

		public void Remove(string robotName)
		{
			var filePath = GetFilePath(robotName);
			if (File.Exists(filePath))
				File.Delete(filePath);
		}

		public void Save(RobotConnectionInfo connection)
		{
			var filePath = GetFilePath(connection.RobotName);
			File.WriteAllText(filePath, JsonConvert.SerializeObject(connection));
		}

		string GetFilePath(string robotName)
		{
			var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);
			root = Path.Combine(root, "Vector");
			if (!Directory.Exists(root))
				Directory.CreateDirectory(root);
			return Path.Combine(root, robotName + ".json");
		}
	}
}
