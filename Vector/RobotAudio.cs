using Anki.Vector.ExternalInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vector
{
	public class RobotAudio : RobotModule
	{
		CancellationTokenSource _cancelAudioFeed;

		public event EventHandler<WakeWordEventArgs> OnWakeWord;

		internal RobotAudio(RobotConnection connection) : base(connection)
		{
		}

		public async Task SayTextAsync(string text, float duration = 1f, bool useVectorVoice = true,  CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.SayTextAsync(new SayTextRequest() { Text = text, DurationScalar = duration, UseVectorVoice = useVectorVoice }, cancellationToken: cancellationToken);
			ValidateStatus(result.Status);
		}

		/// <summary>
		/// Set the master volume level
		/// </summary>
		/// <param name="volume">value between 1 - 5</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task SetMasterVolumeAsync(int volume, CancellationToken cancellationToken = default(CancellationToken))
		{
			//convert to volume level
			var volumeLevel = (MasterVolumeLevel)(((double)volume).Clamp(1, 5) - 1);

			var result = await Client.SetMasterVolumeAsync(new MasterVolumeRequest() { VolumeLevel = volumeLevel }, cancellationToken: cancellationToken);
			ValidateStatus(result.Status);
		}

		public void StartAudioFeed()
		{
			//cancel prev task
			StopAudioFeed();

			//start task
			_cancelAudioFeed = new CancellationTokenSource();
			Task.Run(() => AudioFeedAsync(_cancelAudioFeed.Token));
		}

		public void StopAudioFeed()
		{
			//cancel task
			if (_cancelAudioFeed != null && !_cancelAudioFeed.IsCancellationRequested)
				_cancelAudioFeed.Cancel();
		}

		public async Task AudioFeedAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var stream = Client.AudioFeed(new AudioFeedRequest());
			while (await stream.ResponseStream.MoveNext(cancellationToken))
			{
				var result = stream.ResponseStream.Current;

				//TODO: impliment
			}
		}

		internal void WakeWordEvent(Anki.Vector.ExternalInterface.WakeWord eventData)
		{
			//map entity
			var data = Map<WakeWord>(eventData);

			//send event
			OnWakeWord?.Invoke(this, new WakeWordEventArgs() { Data = data });
		}
	}


	public class WakeWordEventArgs : EventArgs
	{
		public WakeWord Data { get; set; }
	}
}
