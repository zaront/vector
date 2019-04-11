using System;
using System.Collections.Generic;
using System.Text;
using Anki.Vector.ExternalInterface;
using static Anki.Vector.ExternalInterface.ExternalInterface;

namespace Vector
{
	public abstract class RobotModule
	{
		EntityMapper _mapper;
		internal RobotConnection Connection { get; }

		internal RobotModule(RobotConnection connection)
		{
			//set fields
			Connection = connection;
			_mapper = new EntityMapper();
		}

		protected T Map<T>(object source)
		{
			return _mapper.Map<T>(source);
		}

		protected ExternalInterfaceClient Client
		{
			get
			{
				//validate
				var client = Connection.Client;
				if (client == null)
					throw new VectorNotConnectedException("Not connected to Vector.");

				return client;
			}
		}

		protected int GetActionTagID()
		{
			return Connection.GetActionTagID();
		}

		protected void ValidateStatus(ResponseStatus status)
		{
			if (status == null || (status.Code != ResponseStatus.Types.StatusCode.Ok && status.Code != ResponseStatus.Types.StatusCode.RequestProcessing && status.Code != ResponseStatus.Types.StatusCode.ResponseReceived))
				throw new VectorCommandException($"returned error status: {status.Code}");
		}

		protected void ValidateAction(ActionResult result)
		{
			if (result == null || result.Code != ActionResult.Types.ActionResultCode.ActionResultSuccess)
				throw new VectorCommandException($"returned error action: {result.Code}");
		}
	}

}
