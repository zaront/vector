using System;
using System.Collections.Generic;
using System.Text;

namespace Vector
{
	public static class MathExtention
	{
		public static double Clamp(this double source, double min = 0, double max = 1)
		{
			return Math.Min(max, Math.Min(max, source));
		}

		public static double DeadZone(this double source, double deadZoneRange = .05)
		{
			if (source <= deadZoneRange && source >= -deadZoneRange)
				return 0;
			return source;
		}

		public static double Distance(this Vector2 source, Vector2 dest)
		{
			
			return Math.Sqrt(Math.Pow(source.X - dest.X, 2) + Math.Pow(source.Y - dest.Y, 2));
		}

		public static double ToDegrees(this double rad)
		{
			return rad * 180 / Math.PI;
		}
	}
}
