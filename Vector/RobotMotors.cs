using Anki.Vector.ExternalInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vector
{
	public class RobotMotors
	{
		Robot _robot;

		internal RobotMotors(Robot robot)
		{
			//set fields
			_robot = robot;
		}

		public async Task DriveAsync(float leftWheelSpeed, float rightWheelSpeed, float leftWheelAccel = 0f, float rightWheelAccel = 0f, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.DriveWheelsAsync(new DriveWheelsRequest() { LeftWheelMmps = leftWheelSpeed, LeftWheelMmps2 = leftWheelAccel, RightWheelMmps = rightWheelSpeed, RightWheelMmps2 = rightWheelAccel }, cancellationToken: cancellationToken);
			//if (result?.Status?.Code != ResponseStatus.Types.StatusCode.ResponseReceived)
			//	throw new VectorCommunicationException($"communication error: {result?.Status?.Code}");
		}

		public async Task DriveStraightAsync(float distance, float speed, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.DriveStraightAsync(new DriveStraightRequest() { DistMm = distance, SpeedMmps = speed, IdTag = _robot.GetActionTagID() }, cancellationToken: cancellationToken);
		}

		public async Task TurnInPlaceAsync(float angle, float speed, float accel = 1f, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.TurnInPlaceAsync(new TurnInPlaceRequest() {  AngleRad = angle, SpeedRadPerSec = speed, AccelRadPerSec2 = accel, IdTag = _robot.GetActionTagID() }, cancellationToken: cancellationToken);
		}

		public async Task DriveOffChargerAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.DriveOffChargerAsync(new DriveOffChargerRequest(), cancellationToken: cancellationToken);
		}

		public async Task DriveOnChargerAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.DriveOnChargerAsync(new DriveOnChargerRequest(), cancellationToken: cancellationToken);
		}

		public async Task DockWithCubeAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.DockWithCubeAsync(new DockWithCubeRequest(), cancellationToken: cancellationToken);
		}

		public async Task MoveHeadAsync(float speed, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.MoveHeadAsync(new MoveHeadRequest() { SpeedRadPerSec = speed }, cancellationToken: cancellationToken);
		}

		public async Task MoveLiftAsync(float speed, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.MoveLiftAsync(new MoveLiftRequest() { SpeedRadPerSec = speed }, cancellationToken: cancellationToken);
		}

		public async Task MoveHeadToAsync(float angle, float duration = 1f, float accel = 1f, float maxSpeed = 1f, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.SetHeadAngleAsync(new SetHeadAngleRequest() { AngleRad = angle, DurationSec = duration, AccelRadPerSec2 = accel, MaxSpeedRadPerSec = maxSpeed, IdTag = _robot.GetActionTagID() }, cancellationToken: cancellationToken);
		}

		public async Task MoveLiftToAsync(float height, float duration = 1f, float accel = 1f, float maxSpeed = 1f, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.SetLiftHeightAsync(new SetLiftHeightRequest() { HeightMm = height, DurationSec = duration, AccelRadPerSec2 = accel, MaxSpeedRadPerSec = maxSpeed, IdTag = _robot.GetActionTagID() }, cancellationToken: cancellationToken);
		}

	}
}
