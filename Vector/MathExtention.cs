using System;
using System.Collections.Generic;
using System.Numerics;
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

		public static double ToDegrees(this double rad)
		{
			return rad * 180 / Math.PI;
		}

		public static Vector2 ToVector2(this Vector3 source)
		{
			return new Vector2(source.X, source.Y);
		}

		public static Vector3 ToVector3(this Vector2 source)
		{
			return new Vector3(source.X, source.Y, 0);
		}
	}
}
