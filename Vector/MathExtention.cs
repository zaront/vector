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

        public static double ToRadians(this double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static Vector2 ToVector2(this Vector3 source)
		{ 
			return new Vector2(source.X, source.Y);
		}

		public static Vector3 ToVector3(this Vector2 source)
		{
			return new Vector3(source.X, source.Y, 0);
		}

		public static double Distance(this Vector3 source, Vector3 dest)
		{
			return Vector3.Distance(source, dest);
		}

		public static double Distance2D(this Vector3 source, Vector3 dest)
		{
			return Distance(source.ToVector2(), dest.ToVector2());
		}

		public static double Distance(this Vector2 source, Vector2 dest)
		{
			return Vector2.Distance(source, dest);
		}

		/// <summary>
		/// get a caliberated distance from an observed object, to the robots lift
		/// </summary>
		public static double Distance(this ObservedObject observedObject, Vector3 robotTranslation)
		{
			var distance = observedObject.Pose.Translation.Distance2D(robotTranslation);

			//caliberated marker distance
			//y = 0.8119x - 31.938   R² = 1
			distance = 0.8119 * distance - 31.938;

			return distance;
		}
	}
}
