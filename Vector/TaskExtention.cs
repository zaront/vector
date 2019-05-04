using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vector
{
	public static class TaskExtention
	{
		public static void ThrowFeedException(this Task task)
		{
			task.ContinueWith(i =>
			{
				if (i.IsFaulted && i.Exception != null)
				{
					//don't throw if the task was cancelled
					var rpcCanceled = i.Exception.InnerException as RpcException;
					if (rpcCanceled != null && rpcCanceled.Status.StatusCode == StatusCode.Cancelled)
						return;

					throw i.Exception;
				}
					
			}, TaskContinuationOptions.NotOnCanceled);
		}
	}
}
