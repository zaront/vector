using Anki.Vector.ExternalInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vector
{
	public class RobotAnimation
	{
		Robot _robot;
		string[] _animations;

		internal RobotAnimation(Robot robot)
		{
			//set fields
			_robot = robot;
		}

		public async Task<string[]> ListAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_animations == null)
			{
				var result = await _robot.Client.ListAnimationsAsync(new ListAnimationsRequest(), cancellationToken: cancellationToken);
				if (result?.Status?.Code != ResponseStatus.Types.StatusCode.ResponseReceived)
					throw new VectorCommunicationException($"communication error: {result?.Status?.Code}");
				_animations = result.AnimationNames.Select(i => i.Name).ToArray();
			}
			return _animations;
		}

		public async Task PlayAsync(string animationName, bool ignoreBodyTrack = false, bool ignoreHeadTrack = false, bool ignoreLiftTrack = false, int loops = 1, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.PlayAnimationAsync(new PlayAnimationRequest() { Animation = new Animation() { Name = animationName }, IgnoreBodyTrack = ignoreBodyTrack, IgnoreHeadTrack = ignoreHeadTrack, IgnoreLiftTrack = ignoreLiftTrack, Loops = (uint)loops }, cancellationToken: cancellationToken);
		}
	}
}
