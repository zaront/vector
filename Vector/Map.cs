using System;
using System.Collections.Generic;
using System.Text;

namespace Vector
{
	public class Map
	{
		public Vector3 Center { get; set; }
		public float Depth { get; set; }
		public float Size { get; set; }
		public IList<Quad> Quads { get; set; }
	}

	public class Quad
	{

	}

	public enum NavNodeContentType
	{
		NavNodeUnknown = 0,
		NavNodeClearOfObstacle = 1,
		NavNodeClearOfCliff = 2,
		NavNodeObstacleCube = 3,
		NavNodeObstacleProximity = 4,
		NavNodeObstacleProximityExplored = 5,
		NavNodeObstacleUnrecognized = 6,
		NavNodeCliff = 7,
		NavNodeInterestingEdge = 8,
		NavNodeNonInterestingEdge = 9,
	}
}
