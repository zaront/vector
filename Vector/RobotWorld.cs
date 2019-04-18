using Anki.Vector.ExternalInterface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vector
{
	public class RobotWorld : RobotModule
	{
		readonly TimeSpan _objectExpires = TimeSpan.FromSeconds(2.4);
		readonly int _maxObjectCount = 20;

		CancellationTokenSource _cancelNavFeed;
		int _customTypeID;

		public ObservedObject[] ObservedObjects { get; }

		public event EventHandler<RobotMapEventArgs> OnMapChanged;
		public event EventHandler<RobotObjectObservedEventArgs> OnObjectObserved;

		internal RobotWorld(RobotConnection connection) : base(connection)
		{
			//set fields
			ObservedObjects = new ObservedObject[_maxObjectCount + 1];
		}

		public async Task<int> AddWallAsync(ObjectMarker objectMarker, bool isUnique, float markerWidth, float markerHight, float wallWidth, float wallHeight, CancellationToken cancellationToken = default(CancellationToken))
		{
			var customType = GetNextCustomType();
			var result = await Client.DefineCustomObjectAsync(new DefineCustomObjectRequest()
			{
				IsUnique = isUnique,
				CustomType = customType,
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
			ObservedObjects[(int)customType] = new ObservedObject() { ObjectId = (int)customType };
			return (int)customType;
		}

		CustomType GetNextCustomType()
		{
			var next = _customTypeID + 1;

			//allow only 20
			if (next > _maxObjectCount)
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

				//add extended data
				data.LastSeen = DateTime.Now;
				data.IsVisible = true;

				//update local state
				ObservedObjects[data.ObjectId] = data;

				//send event
				OnObjectObserved?.Invoke(this, new RobotObjectObservedEventArgs() { Object = data });
			}
		}

		internal void ObjectEventExpire()
		{
			//expire old objects
			var now = DateTime.Now;
			foreach (var obj in ObservedObjects)
			{
				if (obj != null && obj.LastSeen.Add(_objectExpires) < now && obj.IsVisible)
					obj.IsVisible = false;
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
