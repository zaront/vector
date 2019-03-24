using Anki.Vector.ExternalInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vector
{
	public class RobotScreen
	{
		Robot _robot;

		internal RobotScreen(Robot robot)
		{
			//set fields
			_robot = robot;
		}

		public async Task SetEyeColor(float hue, float saturation, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.SetEyeColorAsync(new SetEyeColorRequest() { Hue = hue, Saturation = saturation }, cancellationToken: cancellationToken);
			if (result?.Status?.Code != ResponseStatus.Types.StatusCode.ResponseReceived)
				throw new VectorCommunicationException($"communication error: {result?.Status?.Code}");
		}

		public async Task SetScreenColor(float hue, float saturation, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.DisplayFaceImageRGBAsync(new DisplayFaceImageRGBRequest() {  }, cancellationToken: cancellationToken);
			if (result?.Status?.Code != ResponseStatus.Types.StatusCode.ResponseReceived)
				throw new VectorCommunicationException($"communication error: {result?.Status?.Code}");
		}


	}
}
