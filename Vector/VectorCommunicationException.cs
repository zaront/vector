using System;
using System.Collections.Generic;
using System.Text;

namespace Vector
{
	public class VectorCommunicationException : Exception
	{
		public VectorCommunicationException(string message) : base(message)
		{
		}
	}
}
