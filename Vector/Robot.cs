using Anki.Vector.ExternalInterface;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Anki.Vector.ExternalInterface.ExternalInterface;

namespace Vector
{
	public class Robot : RobotModule, IDisposable
	{
		CancellationTokenSource _cancelSuppressPersonality;
		CancellationTokenSource _cancelEventListening;
		RobotState _currentState;
		public RobotAudio Audio { get; }
		public RobotMotors Motors { get; }
		public RobotAnimation Animation { get; }
		public RobotScreen Screen { get; }
		public RobotCamera Camera { get; }
		public RobotWorld World { get; }
		public IRobotConnectionInfoStorage ConnectionInfoStorage { get; }

		public event EventHandler<RobotStateEventArgs> OnStateChanged;
		public event EventHandler<SuppressPersonalityEventArgs> OnSuppressPersonality;

		public Robot(IRobotConnectionInfoStorage connectionInfoStorage = null) : base(new RobotConnection())
		{
			//set fields
			ConnectionInfoStorage = connectionInfoStorage ?? new RobotConnectionInfoStorage();
			Audio = new RobotAudio(Connection);
			Motors = new RobotMotors(Connection);
			Animation = new RobotAnimation(Connection);
			Screen = new RobotScreen(Connection);
			Camera = new RobotCamera(Connection);
			World = new RobotWorld(Connection);
		}

		public bool IsConnected { get => Connection.IsConnected; }

		/// <summary>
		/// use IRobotConnectionInfoStorage to retreave connection information to connect to your robot 
		/// </summary>
		/// <param name="robotName">Find your robot name (ex. Vector-A1B2) by placing Vector on the charger and double-clicking Vector's backpack button.</param>
		/// <param name="ipAddress">Update your robots IP address, otherwise leave blank.  Find your robot ip address (ex. 192.168.42.42) by placing Vector on the charger, double-clicking Vector's backpack button, then raising and lowering his arms.If you see XX.XX.XX.XX on his face, reconnect Vector to your WiFi using the Vector Companion App.</param>
		public async Task ConnectAsync(string robotName, string ipAddress = null)
		{
			if (!Connection.IsConnected)
			{
				robotName = ApiAccess.FormatRobotName(robotName);

				//get connection info
				var connectionInfo = ConnectionInfoStorage.Get(robotName);
				if (connectionInfo == null)
				{
					throw new VectorMissingConnectionInfoException("No Connection Info found. (call GrantApiAccessAsync first).  If this is the first time you have connected, you must grant access for this device to communicate with Vector.");
				}
				if (ipAddress != null)
					connectionInfo.IpAddress = ipAddress;

				//connect
				await Connection.ConnectAsync(connectionInfo);

				//update the IP address
				if (ipAddress != null)
					ConnectionInfoStorage.Save(connectionInfo);
			}
		}

		/// <summary>
		/// connect to your robot by passing in a connectionInfo
		/// </summary>
		/// <param name="connectionInfo">call ApiAccess.Grant() to generate a connectionInfo.  Once access is granted the conncetionInfo should be reused.</param>
		/// <returns></returns>
		public async Task ConnectAsync(RobotConnectionInfo connectionInfo)
		{
			await Connection.ConnectAsync(connectionInfo);
		}

		/// <summary>
		/// Required for first time use.  Used to grant this device access to a Vector robot.  The robot must be joined to the wifi network using the Vector Companion App before running this.  Once access is granted you can connect via robotName
		/// </summary>
		/// <param name="robotName">Find your robot name (ex. Vector-A1B2) by placing Vector on the charger and double-clicking Vector's backpack button.</param>
		/// <param name="ipAddress">Find your robot ip address (ex. 192.168.42.42) by placing Vector on the charger, double-clicking Vector's backpack button, then raising and lowering his arms.If you see XX.XX.XX.XX on his face, reconnect Vector to your WiFi using the Vector Companion App.</param>
		/// <param name="serialNumber">Please find your robot serial number (ex. 00e20100) located on the underside of Vector, or accessible from Vector's debug screen.</param>
		/// <param name="userName">Enter your email. Make sure to use the same account that was used to set up your Vector through the Companion app.</param>
		/// <param name="password">Enter your password. Make sure to use the same account that was used to set up your Vector through the Companion app.</param>
		public async Task GrantApiAccessAsync(string robotName, string ipAddress, string serialNumber, string userName, string password)
		{
			//grant API access
			var connectionInfo = await ApiAccess.GrantAsync(robotName, ipAddress, serialNumber, userName, password);

			//save the connection info
			ConnectionInfoStorage.Save(connectionInfo);
		}

		public async Task DisconnectAsync()
		{
			await Connection.DisconnectAsync();
		}

		void IDisposable.Dispose()
		{
			//dispose of connection
			(Connection as IDisposable).Dispose();
		}


		public RobotState CurrentState { get => _currentState; }

		public async Task<BatteryState> GetBatteryStateAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.BatteryStateAsync(new BatteryStateRequest(), cancellationToken: cancellationToken);
			ValidateStatus(result.Status);
			return Map<BatteryState>(result);
		}

		[Obsolete("doesn't appear fully implimented yet")]
		public async Task<NetworkState> GetNetworkStateAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.NetworkStateAsync(new NetworkStateRequest(), cancellationToken: cancellationToken);
			ValidateStatus(result.Status);
			return Map<NetworkState>(result);
		}

		public async Task<VersionState> GetVersionStateAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.VersionStateAsync(new VersionStateRequest(), cancellationToken: cancellationToken);
			ValidateStatus(result.Status);
			return Map<VersionState>(result);
		}

		public void StartSuppressingPersonality(bool overrideSafty = false)
		{
			//cancel prev task
			StopSuppressingPersonality();

			//start task
			_cancelSuppressPersonality = new CancellationTokenSource();
			Task.Run(() => SuppressPersonalityAsync(overrideSafty, _cancelSuppressPersonality.Token));
	}

		public void StopSuppressingPersonality()
		{
			//cancel task
			if (_cancelSuppressPersonality != null && !_cancelSuppressPersonality.IsCancellationRequested)
				_cancelSuppressPersonality.Cancel();
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

		public void StartEventListening()
		{
			//cancel prev task
			StopEventListening();

			//start task
			_cancelEventListening = new CancellationTokenSource();
			Task.Run(() => EventListeningAsync(_cancelEventListening.Token));
		}

		public void StopEventListening()
		{
			//cancel task
			if (_cancelEventListening != null && !_cancelEventListening.IsCancellationRequested)
				_cancelEventListening.Cancel();
		}

		public async Task EventListeningAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var stream = Client.EventStream(new EventRequest() { ConnectionId = Guid.NewGuid().ToString() });
			while (await stream.ResponseStream.MoveNext(cancellationToken))
			{
				var result = stream.ResponseStream.Current;

				//fire event
				switch (result.Event.EventTypeCase)
				{
					case Event.EventTypeOneofCase.TimeStampedStatus:
						//TODO: impliment
						break;
					case Event.EventTypeOneofCase.WakeWord:
						Audio.WakeWordEvent(result.Event.WakeWord);
						break;
					case Event.EventTypeOneofCase.RobotObservedFace:
						//TODO: impliment
						break;
					case Event.EventTypeOneofCase.RobotChangedObservedFaceId:
						//TODO: impliment
						break;
					case Event.EventTypeOneofCase.ObjectEvent:
						World.ObjectEvent(result.Event.ObjectEvent);
						//TODO: impliment
						break;
					case Event.EventTypeOneofCase.StimulationInfo:
						//TODO: impliment
						break;
					case Event.EventTypeOneofCase.PhotoTaken:
						//TODO: impliment
						break;
					case Event.EventTypeOneofCase.RobotState:
						RobotStateEvent(result.Event.RobotState);
						break;
					case Event.EventTypeOneofCase.CubeBattery:
						//TODO: impliment
						break;
					case Event.EventTypeOneofCase.KeepAlive:
						//TODO: impliment
						break;
					case Event.EventTypeOneofCase.ConnectionResponse:
						//TODO: impliment
						break;
					case Event.EventTypeOneofCase.MirrorModeDisabled:
						//TODO: impliment
						break;
					case Event.EventTypeOneofCase.VisionModesAutoDisabled:
						//TODO: impliment
						break;
				}

				World.ObjectEventExpire(); //expire old objects
			}
		}

		void RobotStateEvent(Anki.Vector.ExternalInterface.RobotState eventData)
		{
			//map entity
			var data = Map<RobotState>(eventData);

			//update local state
			_currentState = data;

			//send event
			OnStateChanged?.Invoke(this, new RobotStateEventArgs() { Data = data });
		}



		//public async Task Test2()
		//{
		//	var p = await Client.PhotosInfoAsync(new PhotosInfoRequest());
		//	var r = p.PhotoInfos.ToList();
		//	var p1 = await Client.PhotoAsync(new PhotoRequest() { PhotoId = 3 });
		//	using (var f = System.IO.File.Create(@"C:\Projects\RocDemo\RocClient\test.jpg"))
		//		p1.Image.WriteTo(f);

		//}


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
