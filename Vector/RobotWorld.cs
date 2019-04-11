using Anki.Vector.ExternalInterface;
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
		int _customTypeID;

		public event EventHandler<RobotMapEventArgs> OnMapChanged;
		public event EventHandler<RobotObjectObservedEventArgs> OnObjectObserved;

		internal RobotWorld(RobotConnection connection) : base(connection)
		{
		}

		public async Task AddWallAsync(ObjectMarker objectMarker, bool isUnique, float markerWidth, float markerHight, float wallWidth, float wallHeight, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.DefineCustomObjectAsync(new DefineCustomObjectRequest()
			{
				IsUnique = isUnique,
				CustomType = GetNextCustomType(),
				CustomWall = new CustomWallDefinition()
				{
					HeightMm = wallHeight,
					WidthMm = wallWidth,
					MarkerHeightMm = markerHight,
					MarkerWidthMm = markerWidth,
					Marker = (CustomObjectMarker)objectMarker
				}
			});
			ValidateStatus(result.Status);
			SaveNextCustomType();
		}

		CustomType GetNextCustomType()
		{
			var next = _customTypeID + 1;

			//allow only 20
			if (next > 20)
				throw new VectorArgumentException("max quantity of object tracking reached");

			return (CustomType)next;
		}

		void SaveNextCustomType()
		{
			_customTypeID++;
		}

		public void StartMapFeed(TimeSpan updateFrequency = default(TimeSpan))
		{
			//cancel prev task
			StopMapFeed();

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

		public async Task MapFeedAsync(TimeSpan updateFrequency = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken))
		{
			//default map update frequency
			if (updateFrequency == default(TimeSpan))
				updateFrequency = TimeSpan.FromSeconds(1);
			
			var stream = Client.NavMapFeed(new NavMapFeedRequest() { Frequency = (float)updateFrequency.TotalSeconds });
			while (await stream.ResponseStream.MoveNext(cancellationToken))
			{
				var result = stream.ResponseStream.Current;

				//TODO: impliment
				var map = Map<Map>(result);

				//send event
				OnMapChanged?.Invoke(this, new RobotMapEventArgs() { Map = map });
			}
		}

		internal void ObjectEvent(ObjectEvent eventData)
		{
			if (eventData.ObjectEventTypeCase == Anki.Vector.ExternalInterface.ObjectEvent.ObjectEventTypeOneofCase.RobotObservedObject)
			{
				//map entity
				var data = Map<ObservedObject>(eventData.RobotObservedObject);

				//update local state
				//_currentState = data;

				//send event
				OnObjectObserved?.Invoke(this, new RobotObjectObservedEventArgs() { Object = data });
			}
		}
	}



	public class RobotMapEventArgs : EventArgs
	{
		public Map Map { get; set; }
	}
	public class RobotObjectObservedEventArgs : EventArgs
	{
		public ObservedObject Object { get; set; }
	}
}
