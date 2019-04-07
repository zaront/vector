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

		/// <summary>
		/// set the speed of the wheels
		/// </summary>
		/// <param name="leftWheelSpeed">250 to -250 in mm/s</param>
		/// <param name="rightWheelSpeed">250 to -250 in mm/s</param>
		/// <param name="leftWheelAccel">250 to -250 in mm/s^2</param>
		/// <param name="rightWheelAccel">250 to -250 in mm/s^2</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
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
			var result = await _robot.Client.TurnInPlaceAsync(new TurnInPlaceRequest() { AngleRad = angle, SpeedRadPerSec = speed, AccelRadPerSec2 = accel, IdTag = _robot.GetActionTagID() }, cancellationToken: cancellationToken);
		}

		public async Task DriveOffChargerAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.DriveOffChargerAsync(new DriveOffChargerRequest(), cancellationToken: cancellationToken);
		}

		public async Task DriveOnChargerAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.DriveOnChargerAsync(new DriveOnChargerRequest(), cancellationToken: cancellationToken);
		}

		ConnectCubeResponse _cube;
		public async Task<bool> DockWithCubeAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			//if (_cube == null)
			//	_cube = await _robot.Client.ConnectCubeAsync(new ConnectCubeRequest(), cancellationToken: cancellationToken);
			//if (_cube.ObjectId == 0)
			//	return false;

			var result = await _robot.Client.DockWithCubeAsync(new DockWithCubeRequest() { IdTag = _robot.GetActionTagID(), ObjectId = 1 }, cancellationToken: cancellationToken);
			return (result.Result.Code == ActionResult.Types.ActionResultCode.ActionResultSuccess);
		}

		public async Task MoveHeadAsync(float speed, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _robot.Client.MoveHeadAsync(new MoveHeadRequest() { SpeedRadPerSec = speed }, cancellationToken: cancellationToken);
		}

		/// <param name="speed">rad/s</param>
		/// <returns></returns>
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
