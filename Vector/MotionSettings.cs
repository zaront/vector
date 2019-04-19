using System;
using System.Collections.Generic;
using System.Text;

namespace Vector
{
	public class MotionSettings
	{
		public float SpeedMmps { get; set; }
		public float AccelMmps2 { get; set; }
		public float DecelMmps2 { get; set; }
		public float PointTurnSpeedRadPerSec { get; set; }
		public float PointTurnAccelRadPerSec2 { get; set; }
		public float PointTurnDecelRadPerSec2 { get; set; }
		public float DockSpeedMmps { get; set; }
		public float DockAccelMmps2 { get; set; }
		public float DockDecelMmps2 { get; set; }
		public float ReverseSpeedMmps { get; set; }
		public bool IsCustom { get; set; }
	}
}
