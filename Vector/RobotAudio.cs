using Anki.Vector.ExternalInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vector
{
	public class RobotAudio
	{
		Robot _robot;

		internal RobotAudio(Robot robot)
		{
			//set fields
			_robot = robot;
		}

		public async Task SayTextAsync(string text, float duration = 1f, bool useVectorVoice = true,  CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.SayTextAsync(new SayTextRequest() { Text = text, DurationScalar = duration, UseVectorVoice = useVectorVoice }, cancellationToken: cancellationToken);
			if (result?.Status?.Code != ResponseStatus.Types.StatusCode.ResponseReceived)
				throw new VectorCommunicationException($"communication error: {result?.Status?.Code}");
		}
	}
}
