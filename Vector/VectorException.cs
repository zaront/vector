using System;
using System.Collections.Generic;
using System.Text;

namespace Vector
{
	//root exception
	public class VectorException : Exception
	{
		public VectorException(string message) : base(message){}
		public VectorException(string message, Exception innerException) : base(message, innerException) { }
	}



	//connection exceptions
	public class VectorConnectionException : VectorException //base
	{
		public VectorConnectionException(string message) : base(message) { }
		public VectorConnectionException(string message, Exception innerException) : base(message, innerException) { }
	}

	public class VectorNotConnectedException : VectorConnectionException
	{
		public VectorNotConnectedException(string message) : base(message) { }
		public VectorNotConnectedException(string message, Exception innerException) : base(message, innerException) { }
	}

	public class VectorMissingConnectionInfoException : VectorConnectionException
	{
		public VectorMissingConnectionInfoException(string message) : base(message) { }
		public VectorMissingConnectionInfoException(string message, Exception innerException) : base(message, innerException) { }
	}

	public class VectorAuthorizationException : VectorConnectionException
	{
		public VectorAuthorizationException(string message) : base(message) { }
		public VectorAuthorizationException(string message, Exception innerException) : base(message, innerException) { }
	}



	//command exceptions
	public class VectorCommandException : VectorException //base
	{
		public VectorCommandException(string message) : base(message) { }
		public VectorCommandException(string message, Exception innerException) : base(message, innerException) { }
	}

}
