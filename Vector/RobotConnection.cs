using Anki.Vector.ExternalInterface;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using static Anki.Vector.ExternalInterface.ExternalInterface;

namespace Vector
{
	internal class RobotConnection : IDisposable
	{
		Channel _channel;
		int _actionTagID;

		public ExternalInterfaceClient Client { get; private set; }
		public bool IsConnected { get; private set; }

		void IDisposable.Dispose()
		{
			DisconnectAsync().Wait();
		}

		public async Task ConnectAsync(RobotConnectionInfo connectionInfo)
		{
			if (Client == null)
			{
				//create channel
				var ssl = new SslCredentials(connectionInfo.Certificate);
				var interceptor = new AsyncAuthInterceptor((context, metadata) =>
				{
					metadata.Add("authorization", $"Bearer {connectionInfo.Token}");
					return Task.CompletedTask;
				});
				var cred = ChannelCredentials.Create(ssl, CallCredentials.FromInterceptor(interceptor));
				_channel = new Channel(connectionInfo.IpAddress, 443, cred, new ChannelOption[] { new ChannelOption("grpc.ssl_target_name_override", connectionInfo.RobotName) });

				//connect to client
				try
				{
					await _channel.ConnectAsync(DateTime.UtcNow.AddSeconds(10));
				}
				catch (TaskCanceledException ex)
				{
					throw new VectorConnectionException("could not connect to Vector.  insure IP address is correct and that Vector is turned on", ex);
				}

				//create client
				Client = new ExternalInterfaceClient(_channel);
				IsConnected = true;
			}
		}

		public async Task DisconnectAsync()
		{
			if (Client != null)
			{
				await _channel.ShutdownAsync();
				Client = null;
				IsConnected = false;
			}
		}

		public int GetActionTagID()
		{
			if (_actionTagID == (int)ActionTagConstants.InvalidSdkTag)
				_actionTagID = (int)ActionTagConstants.FirstSdkTag;
			else
				_actionTagID++;
			if (_actionTagID > (int)ActionTagConstants.LastSdkTag)
				_actionTagID = (int)ActionTagConstants.FirstSdkTag;
			return _actionTagID;
		}

	}
}
