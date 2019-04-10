using Anki.Vector.ExternalInterface;
using AutoMapper;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Anki.Vector.ExternalInterface.ExternalInterface;

namespace Vector
{
	internal class RobotConnection : IDisposable
	{
		static bool _mappingInit;
		Channel _channel;
		int _actionTagID;
		ExternalInterfaceClient _client;

		public bool IsConnected { get; private set; }
		public ExternalInterfaceClient Client { get => _client; }

		public RobotConnection()
		{
			//setup automapper for mapping to results
			if (!_mappingInit)
			{
				Mapper.Initialize(i =>
				{
					i.CreateMap<BatteryStateResponse, BatteryState>();

					i.CreateMap<NetworkStateResponse, NetworkState>();

					i.CreateMap<VersionStateResponse, VersionState>();

					i.CreateMap<Anki.Vector.ExternalInterface.WakeWord, WakeWord>()
						.ForMember(d => d.IntentHeard, m => m.MapFrom(s => s.WakeWordEnd.IntentHeard))
						.ForMember(d => d.IntentJson, m => m.MapFrom(s => s.WakeWordEnd.IntentJson))
						.ForMember(d => d.Begin, m => m.MapFrom(s => s.WakeWordBegin != null));

					i.CreateMap<RobotState, Anki.Vector.ExternalInterface.RobotState>();

					//i.CreateMap<Anki.Vector.ExternalInterface.NavMapFeedResponse, Map>()
					//	.ForMember(d => d, m => m.MapFrom(s => s.MapInfo))
					//	.ForMember(d => d.Quads, m => m.MapFrom(s => s.QuadInfos));
				});
				_mappingInit = true;
			}
		}

		void IDisposable.Dispose()
		{
			DisconnectAsync().Wait();
		}

		public async Task ConnectAsync(RobotConnectionInfo connectionInfo)
		{
			if (_client == null)
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
				_client = new ExternalInterfaceClient(_channel);
				IsConnected = true;
			}
		}

		public async Task DisconnectAsync()
		{
			if (_client != null)
			{
				await _channel.ShutdownAsync();
				_client = null;
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
