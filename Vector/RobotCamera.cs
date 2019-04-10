using Anki.Vector.ExternalInterface;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Vector
{
	public class RobotCamera : RobotModule
	{
		CancellationTokenSource _cancelCameraFeed;

		public event EventHandler<RobotImageEventArgs> ImageReceived;

		internal RobotCamera(RobotConnection connection) : base(connection)
		{
		}

		public void StartCameraFeed()
		{
			//cancel prev feed
			StopCameraFeed();

			//start task
			_cancelCameraFeed = new CancellationTokenSource();
			Task.Run(() => CameraFeedAsync(_cancelCameraFeed.Token));
		}

		public void StopCameraFeed()
		{
			//cancel camera feed task
			if (_cancelCameraFeed != null && !_cancelCameraFeed.IsCancellationRequested)
				_cancelCameraFeed.Cancel();
		}

		async Task CameraFeedAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			//get camera images
			var cameraFeed = Client.CameraFeed(new CameraFeedRequest());
			while(await cameraFeed.ResponseStream.MoveNext(_cancelCameraFeed.Token))
			{
				var response = cameraFeed.ResponseStream.Current;
				if (response.Data != null && !response.Data.IsEmpty)
				{
					//convert to image
					Image image = null;
					using (var stream = new MemoryStream(response.Data.ToByteArray()))
						image = Image.FromStream(stream);

					//send event
					ImageReceived?.Invoke(this, new RobotImageEventArgs() { Image = image });
				}
			}
		}
	}



	public class RobotImageEventArgs : EventArgs
	{
		public Image Image { get; set; }
	}
}
