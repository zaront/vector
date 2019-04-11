using System;
using System.Collections.Generic;
using System.Text;

namespace Vector
{
	public class ObservedObject
	{
		public uint IsActive { get; set; }
		public ObjectFamily ObjectFamily { get; set; }
		public int ObjectId { get; set; }
		public ObjectType ObjectType { get; set; }
		public Pose Pose { get; set; }
		public float TopFaceOrientationRad { get; set; }
		public Rect ImgRect { get; set; }
	}

	public enum ObjectFamily
	{
		InvalidFamily = 0,
		UnknownFamily = 1,
		Block = 2,
		LightCube = 3,
		Charger = 4,
		CustomObject = 7
	}

	public enum ObjectType
	{
		InvalidObject = 0,
		UnknownObject = 1,
		BlockLightcube1 = 2,
		FirstCustomObjectType = 15
	}

	public class Rect
	{
		public float Height { get; set; }
		public float Width { get; set; }
		public float XTopLeft { get; set; }
		public float YTopLeft { get; set; }
	}
}
