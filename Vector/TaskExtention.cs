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
					throw i.Exception;
			});
		}
	}
}
