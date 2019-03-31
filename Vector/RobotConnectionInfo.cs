using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Vector
{
	public class RobotConnectionInfo
	{
		/// <summary>
		/// call Robot.GrantApiAccess() or ApiAccess.Grant() to get this the first time
		/// </summary>
		public string Token;
		/// <summary>
		/// call Robot.GrantApiAccess() or ApiAccess.Grant() to get this the first time
		/// </summary>
		public string Certificate;
		public string RobotName;
		public string IpAddress;
	}
}
