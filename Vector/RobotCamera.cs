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
		public const int CameraWidth = 360;
		public const int CameraHeight = 640;
		public const int CameraInternalWidth = CameraWidth / 2;
		public const int CameraInternalHeight = CameraHeight / 2;

		CancellationTokenSource _cancelCameraFeed;

		public event EventHandler<RobotImageEventArgs> OnImageReceived;

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

		public async Task CameraFeedAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			//get camera images
			var cameraFeed = Client.CameraFeed(new CameraFeedRequest());
			while(await cameraFeed.ResponseStream.MoveNext(cancellationToken))
			{
				var response = cameraFeed.ResponseStream.Current;
				if (response.Data != null && !response.Data.IsEmpty)
				{
					//convert to image
					Image image = null;
					using (var stream = new MemoryStream(response.Data.ToByteArray()))
						image = Image.FromStream(stream);

					//send event
					OnImageReceived?.Invoke(this, new RobotImageEventArgs() { Image = image });
				}
			}
		}
	}



	public class RobotImageEventArgs : EventArgs
	{
		public Image Image { get; set; }
	}
}
