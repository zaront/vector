using System;
using System.Collections.Generic;
using System.Text;

namespace Vector
{
	public class BatteryState
	{
		//ResponseStatus status = 1;
		public BatteryLevel BatteryLevel;
		public float BatteryVolts;
		public bool IsCharging;
		public bool IsOnChargerPlatform;
		public float SuggestedChargerSec;
		public CubeBattery CubeBattery;
	}

	public enum BatteryLevel
	{
		Unknown = 0,
		Low = 1,
		Nominal = 2,
		Full = 3
	}

	public class CubeBattery
	{
		CubeBatteryLevel Level;
		string FactoryID;
		float BatteryVolts;
		float TimeSinceLastReadingSec;
	}

	public enum CubeBatteryLevel
	{
		Low = 0,
		Normal = 1
	}
}
