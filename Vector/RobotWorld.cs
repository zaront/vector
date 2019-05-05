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
		readonly int _firstCustomObject = 15;
		ConcurrentDictionary<int, string> _customNames;

		CancellationTokenSource _cancelNavFeed;
		int _customTypeID;

		public ObservedObject[] ObservedObjects { get; }
		public ObservedObject ObservedCharger { get => ObservedObjects[0]; }
		public ObservedObject ObservedCube { get => ObservedObjects[1]; }

		public event EventHandler<RobotMapEventArgs> OnMapChanged;
		public event EventHandler<RobotObjectObservedEventArgs> OnObjectObserved;
		public event EventHandler<RobotObjectObservedEventArgs> OnObjectExpired;

		internal RobotWorld(RobotConnection connection) : base(connection)
		{
			//set fields
			ObservedObjects = new ObservedObject[_maxObjectCount + 2];
			ObservedObjects[0] = new ObservedObject() { indexId = 0 }; //default charger
			ObservedObjects[1] = new ObservedObject() { indexId = 1 }; //default cube
			_customNames = new ConcurrentDictionary<int, string>();
		}

		public async Task RemoveAllMarkersAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			//remove markers from robot
			var result = await Client.DeleteCustomObjectsAsync(new DeleteCustomObjectsRequest() { Mode = CustomObjectDeletionMode.DeletionMaskCustomMarkerObjects });
			ValidateStatus(result.Status);

			//reset markers in world
			_customTypeID = 0;
			for (int i = 2; i < ObservedObjects.Length; i++)
				ObservedObjects[i] = null;
			_customNames.Clear();
		}

		public async Task<int> AddWallMarkerAsync(string name, ObjectMarker objectMarker, bool isUnique, float markerWidth, float markerHight, float wallWidth, float wallHeight, CancellationToken cancellationToken = default(CancellationToken))
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
			var index = (int)customType + 1;
			ObservedObjects[index] = new ObservedObject() { indexId = index };
			_customNames.AddOrUpdate(index, name, (i, e) => name);
			return index;
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
				switch (data.ObjectFamily)
				{
					case ObjectFamily.Charger:
						data.indexId = 0;
						data.Name = "Charger";
						break;
					case ObjectFamily.LightCube:
						data.indexId = 1;
						data.Name = "Cube";
						break;
					default:
						var index = (int)data.ObjectType;
						if (index >= _firstCustomObject)
							index = index - _firstCustomObject + 2;
						data.indexId = index;
						if (_customNames.TryGetValue(index, out var name))
							data.Name = name;
						break;
				}

				//update local state
				ObservedObjects[data.indexId] = data;

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
				{
					obj.IsVisible = false;

					//send event
					OnObjectExpired?.Invoke(this, new RobotObjectObservedEventArgs() { Object = obj });
				}
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
