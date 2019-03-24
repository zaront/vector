using Anki.Vector.ExternalInterface;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Anki.Vector.ExternalInterface.ExternalInterface;
using AutoMapper;

namespace Vector
{
	public class Robot : IDisposable
	{
		static bool _mappingInit;
		internal ExternalInterfaceClient Client { get; private set; }
		Channel _channel;
		int _actionTagID;

		public RobotAudio Audio { get; }
		public RobotMotors Motors { get; }
		public RobotAnimation Animation { get; }
		public RobotScreen Screen { get; }

		public event EventHandler<EventArgs> OnAnyEvent;
		public event EventHandler<WakeWordEventArgs> OnWakeWord;
		public event EventHandler<RobotStateEventArgs> OnStateChanged;
		public event EventHandler<SuppressPersonalityEventArgs> OnSuppressPersonality;

		public Robot()
		{
			//setup mapping
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

				});
				_mappingInit = true;
			}

			//set fields
			Audio = new RobotAudio(this);
			Motors = new RobotMotors(this);
			Animation = new RobotAnimation(this);
			Screen = new RobotScreen(this);
		}

		internal int GetActionTagID()
		{
			if (_actionTagID == (int)ActionTagConstants.InvalidSdkTag)
				_actionTagID = (int)ActionTagConstants.FirstSdkTag;
			else
				_actionTagID++;
			if (_actionTagID > (int)ActionTagConstants.LastSdkTag)
				_actionTagID = (int)ActionTagConstants.FirstSdkTag;
			return _actionTagID;
		}

		public async Task ConnectAsync(RobotConfiguration robotConfiguration)
		{
			if (Client == null)
			{
				//create channel
				var sslCredentials = new SslCredentials(robotConfiguration.Certificate);
				var channelCredentials = ChannelCredentials.Create(
					sslCredentials,
					CallCredentials.FromInterceptor(
						_GetAsyncAuthInterceptorFromAccessToken(robotConfiguration.Guid)
					)
				);
				_channel = new Channel(
					robotConfiguration.Target,
					channelCredentials,
					new ChannelOption[]
					{
				new ChannelOption("grpc.ssl_target_name_override", robotConfiguration.RobotName)
					}
				);

				//create client
				Client = new ExternalInterfaceClient(_channel);

				//connect to client
				await _channel.ConnectAsync();
			}
		}

		public async Task DisconnectAsync()
		{
			if (Client != null)
			{
				await _channel.ShutdownAsync();
				Client = null;
			}
		}

		public void Dispose()
		{
			DisconnectAsync().Wait();
		}

		public async Task<BatteryState> GetBatteryStateAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.BatteryStateAsync(new BatteryStateRequest(), cancellationToken: cancellationToken);
			if (result?.Status?.Code == ResponseStatus.Types.StatusCode.ResponseReceived)
				return Mapper.Map<BatteryState>(result);
			throw new VectorCommunicationException($"communication error: {result?.Status?.Code}");
		}

		[Obsolete("doesn't appear fully implimented yet")]
		public async Task<NetworkState> GetNetworkStateAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.NetworkStateAsync(new NetworkStateRequest(), cancellationToken: cancellationToken);
			if (result?.Status?.Code == ResponseStatus.Types.StatusCode.ResponseReceived)
				return Mapper.Map<NetworkState>(result);
			throw new VectorCommunicationException($"communication error: {result?.Status?.Code}");
		}

		public async Task<VersionState> GetVersionStateAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.VersionStateAsync(new VersionStateRequest(), cancellationToken: cancellationToken);
			if (result?.Status?.Code == ResponseStatus.Types.StatusCode.ResponseReceived)
				return Mapper.Map<VersionState>(result);
			throw new VectorCommunicationException($"communication error: {result?.Status?.Code}");
		}

		public async Task StartEventListeningAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var stream = Client.EventStream(new EventRequest() { ConnectionId = Guid.NewGuid().ToString() });
			while (await stream.ResponseStream.MoveNext(cancellationToken))
			{
				var result = stream.ResponseStream.Current;

				//fire event
				switch (result.Event.EventTypeCase)
				{
					case Event.EventTypeOneofCase.TimeStampedStatus:
						break;
					case Event.EventTypeOneofCase.WakeWord:
						var e1 = new WakeWordEventArgs() { Data = Mapper.Map<WakeWord>(result.Event.WakeWord) };
						OnWakeWord?.Invoke(this, e1);
						OnAnyEvent?.Invoke(this, e1);
						break;
					case Event.EventTypeOneofCase.RobotObservedFace:
						break;
					case Event.EventTypeOneofCase.RobotChangedObservedFaceId:
						break;
					case Event.EventTypeOneofCase.ObjectEvent:
						break;
					case Event.EventTypeOneofCase.StimulationInfo:
						break;
					case Event.EventTypeOneofCase.PhotoTaken:
						break;
					case Event.EventTypeOneofCase.RobotState:
							var e2 = new RobotStateEventArgs() { Data = Mapper.Map<RobotState>(result.Event.RobotState) };
							OnStateChanged?.Invoke(this, e2);
							OnStateChanged?.Invoke(this, e2);
						break;
					case Event.EventTypeOneofCase.CubeBattery:
						break;
					case Event.EventTypeOneofCase.KeepAlive:
						break;
					case Event.EventTypeOneofCase.ConnectionResponse:
						break;
					case Event.EventTypeOneofCase.MirrorModeDisabled:
						break;
					case Event.EventTypeOneofCase.VisionModesAutoDisabled:
						break;
				}
			}
		}

		public async Task SuppressPersonalityAsync(bool overrideSafty = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			var stream = Client.BehaviorControl();
			var priority = overrideSafty ? ControlRequest.Types.Priority.OverrideAll : ControlRequest.Types.Priority.TopPriorityAi;
			await stream.RequestStream.WriteAsync(new BehaviorControlRequest() { ControlRequest = new ControlRequest() { Priority = priority } });
			while(await stream.ResponseStream.MoveNext(cancellationToken))
			{
				var result = stream.ResponseStream.Current;
				if (result.ControlLostEvent != null)
				{
					OnSuppressPersonality?.Invoke(this, new SuppressPersonalityEventArgs() { IsSuppressed = false });
					await stream.RequestStream.WriteAsync(new BehaviorControlRequest() { ControlRequest = new ControlRequest() { Priority = priority } });
				}
				else if (result.ControlGrantedResponse != null)
					OnSuppressPersonality?.Invoke(this, new SuppressPersonalityEventArgs() { IsSuppressed = true });
			}
		}

		//public async Task ChangeBehaviorAsync(float frequency = .5f, CancellationToken cancellationToken = default(CancellationToken))
		//{
		//	var stream = Client.NavMapFeed(new NavMapFeedRequest() { Frequency = frequency });
		//	while (await stream.ResponseStream.MoveNext(cancellationToken))
		//	{
		//		var result = stream.ResponseStream.Current;
		//		if (result. != null)
		//			await stream.RequestStream.WriteAsync(new BehaviorControlRequest() { ControlRequest = new ControlRequest() { Priority = (ControlRequest.Types.Priority)500 } });
		//	}
		//}



		public async Task Test2()
		{
			var p = await Client.PhotosInfoAsync(new PhotosInfoRequest());
			var r = p.PhotoInfos.ToList();
			var p1 = await Client.PhotoAsync(new PhotoRequest() { PhotoId = 3 });
			using (var f = System.IO.File.Create(@"C:\Projects\RocDemo\RocClient\test.jpg"))
				p1.Image.WriteTo(f);

		}

		public async Task Test()
		{
			var p = await Client.VersionStateAsync(new VersionStateRequest());
			var p2 = await Client.ProtocolVersionAsync(new ProtocolVersionRequest());
			var p3 = await Client.ListAnimationsAsync(new ListAnimationsRequest());
			//var p4 = await _client.DisplayFaceImageRGBAsync(new DisplayFaceImageRGBRequest() { DurationMs = 5000, InterruptRunning = true, FaceData = Google.Protobuf.ByteString.CopyFrom(System.IO.File.ReadAllBytes(@"C:\Projects\RocDemo\RocClient\test.jpg")) });
			var id = Guid.NewGuid().ToString();
			var tt = new EventRequest() { ConnectionId = id };
			var c = new CancellationTokenSource();
			var p5 = Client.EventStream(tt, cancellationToken: c.Token);
			while (await p5.ResponseStream.MoveNext())
			{
				var d = p5.ResponseStream.Current;
				var data = d.Event?.RobotState?.ProxData;
				if (data != null)
					Console.WriteLine(data);
				//c.Cancel();
			}
		}

		public async Task Test4()
		{
			var c = new CancellationTokenSource();
			var p = Client.NavMapFeed(new NavMapFeedRequest() { Frequency = 0.5f, }, cancellationToken: c.Token);
			while (await p.ResponseStream.MoveNext())
			{
				var d = p.ResponseStream.Current;
			}
		}

		public async Task Test5()
		{
			var p2 = Client.AssumeBehaviorControl(new BehaviorControlRequest() { ControlRequest = new ControlRequest() { Priority = ControlRequest.Types.Priority.OverrideAll } });
			while (await p2.ResponseStream.MoveNext())
			{
				var d = p2.ResponseStream.Current;
			}
			//var p = _client.BehaviorControl();
			//while(await p.ResponseStream.MoveNext())
			//{
			//	var d = p.ResponseStream.Current;
			//}
		}

		public async Task Test6()
		{
			var p2 = Client.CameraFeed(new CameraFeedRequest());
			while (await p2.ResponseStream.MoveNext())
			{
				var d = p2.ResponseStream.Current;
			}

			var p = Client.AudioFeed(new AudioFeedRequest());
			while (await p.ResponseStream.MoveNext())
			{
				var d = p.ResponseStream.Current;
			}
		}

		public async Task Test7()
		{
			//var p = _client.PlayAnimationAsync(new PlayAnimationRequest() {  Animation = new Animation() { Name =  } })
		}

		private AsyncAuthInterceptor _GetAsyncAuthInterceptorFromAccessToken(string token)
		{
			return new AsyncAuthInterceptor((context, metadata) =>
			{
				metadata.Add("authorization", $"Bearer {token}");
				return Task.CompletedTask;
			});
		}
	}


	public class AnyEventArgs : EventArgs
	{
		public object Data { get; set; }
	}
	public class WakeWordEventArgs : EventArgs
	{
		public WakeWord Data { get; set; }
	}
	public class RobotStateEventArgs : EventArgs
	{
		public RobotState Data { get; set; }
	}
	public class SuppressPersonalityEventArgs : EventArgs
	{
		public bool IsSuppressed { get; set; }
	}
}
