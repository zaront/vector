using Anki.Vector.ExternalInterface;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vector
{
	public class RobotWorld : RobotModule
	{
		CancellationTokenSource _cancelNavFeed;
		bool _enableCustomMarkerDetection;

		public event EventHandler<RobotMapEventArgs> MapChanged;

		internal RobotWorld(RobotConnection connection) : base(connection)
		{
		}

		public bool EnableCustomMarkerDetection
		{
			get { return _enableCustomMarkerDetection; }
			set
			{
				if (_enableCustomMarkerDetection != value)
				{
					Client.EnableMarkerDetection(new EnableMarkerDetectionRequest() { Enable = value });
					_enableCustomMarkerDetection = value;

					if (value)
					{
						var t = Client.DefineCustomObject(new DefineCustomObjectRequest() { IsUnique = true, CustomWall = new CustomWallDefinition() { HeightMm = 50, WidthMm = 50, MarkerHeightMm = 30, MarkerWidthMm = 30, Marker = CustomObjectMarker.CustomMarkerCircles2 } });
					}
				}
			}
		}
		
		public void StartMapFeed(TimeSpan updateFrequency = default(TimeSpan))
		{
			//cancel prev task
			StopMapFeed();

			//default map update frequency
			if (updateFrequency == default(TimeSpan))
				updateFrequency = TimeSpan.FromSeconds(0.5);

			//start task
			_cancelNavFeed = new CancellationTokenSource();
			Task.Run(() => MapFeedAsync(updateFrequency, _cancelNavFeed.Token));
		}

		public void StopMapFeed()
		{
			//cancel task
			if (_cancelNavFeed != null && !_cancelNavFeed.IsCancellationRequested)
				_cancelNavFeed.Cancel();
		}

		async Task MapFeedAsync(TimeSpan updateFrequency, CancellationToken cancellationToken = default(CancellationToken))
		{
			var stream = Client.NavMapFeed(new NavMapFeedRequest() { Frequency = (float)updateFrequency.TotalSeconds });
			while (await stream.ResponseStream.MoveNext(cancellationToken))
			{
				var result = stream.ResponseStream.Current;

				//TODO: impliment
				//var map = Mapper.Map<Map>(result);

				//send event
				//MapChanged?.Invoke(this, new RobotMapEventArgs() { Map = map });
			}
		}
	}



	public class RobotMapEventArgs : EventArgs
	{
		public Map Map { get; set; }
	}
}
