using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Vector
{
	public class Map
	{
		public MapInfo MapInfo { get; set; }
		public IList<Quad> QuadInfos { get; set; }
		public uint OriginId { get; set; }
	}

	public class MapInfo
	{
		public float RootCenterX { get; set; }
		public float RootCenterY { get; set; }
		public float RootCenterZ { get; set; }
		public int RootDepth { get; set; }
		public float RootSizeMm { get; set; }
	}

	public class Quad
	{
		public uint ColorRgba { get; set; }
		public uint Depth { get; set; }
		public MapNodeType Content { get; set; }
	}

	public enum MapNodeType
	{
		Unknown = 0,
		ClearOfObstacle = 1,
		ClearOfCliff = 2,
		ObstacleCube = 3,
		ObstacleProximity = 4,
		ObstacleProximityExplored = 5,
		ObstacleUnrecognized = 6,
		Cliff = 7,
		InterestingEdge = 8,
		NonInterestingEdge = 9,
	}
}
