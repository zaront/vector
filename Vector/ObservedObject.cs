using System;
using System.Collections.Generic;
using System.Drawing;
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
		public RectangleF ImgRect { get; set; }

		//extended properties
		public bool IsVisible { get; set; }
		public DateTime LastSeen { get; set; }
		public string Name { get; set; }
		public int indexId { get; set; }
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
}
