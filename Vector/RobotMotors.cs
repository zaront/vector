using Anki.Vector.ExternalInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Vector
{
	public class RobotMotors : RobotModule
	{
		Robot _robot;
		internal RobotMotors(RobotConnection connection, Robot robot) : base(connection)
		{
			//set fields
			_robot = robot;
		}

		public MotionSettings DefaultMotionSettings { get; set; }

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
			var result = await Client.DriveWheelsAsync(new DriveWheelsRequest() { LeftWheelMmps = leftWheelSpeed, LeftWheelMmps2 = leftWheelAccel, RightWheelMmps = rightWheelSpeed, RightWheelMmps2 = rightWheelAccel }, cancellationToken: cancellationToken);
			//if (result?.Status?.Code != ResponseStatus.Types.StatusCode.ResponseReceived)
			//	throw new VectorCommunicationException($"communication error: {result?.Status?.Code}");
		}

		public async Task DriveStraightAsync(float distance, float speed, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.DriveStraightAsync(new DriveStraightRequest() { DistMm = distance, SpeedMmps = speed, IdTag = GetActionTagID() }, cancellationToken: cancellationToken);
		}

		public async Task TurnInPlaceAsync(float angle, float speed, float accel = 1f, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.TurnInPlaceAsync(new TurnInPlaceRequest() { AngleRad = angle, SpeedRadPerSec = speed, AccelRadPerSec2 = accel, IdTag = GetActionTagID() }, cancellationToken: cancellationToken);
		}

		public async Task DriveOffChargerAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.DriveOffChargerAsync(new DriveOffChargerRequest(), cancellationToken: cancellationToken);
		}

		public async Task DriveOnChargerAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.DriveOnChargerAsync(new DriveOnChargerRequest(), cancellationToken: cancellationToken);
		}

		public async Task<bool> DockWithCubeAsync(MotionSettings motionSettings = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			//get visible cube id
			var cubeID = _robot.World.ObservedObjects.Where(i => i != null && i.IsVisible && i.ObjectType == ObjectType.BlockLightcube1).Select(i => i.ObjectId).FirstOrDefault();

			var motion = Map<PathMotionProfile>(motionSettings ?? DefaultMotionSettings);
			var result = await Client.DockWithCubeAsync(new DockWithCubeRequest() { IdTag = GetActionTagID(), ObjectId = cubeID, MotionProf = motion }, cancellationToken: cancellationToken);
			return (result.Result.Code == ActionResult.Types.ActionResultCode.ActionResultSuccess);
		}

		public async Task MoveHeadAsync(float speed, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.MoveHeadAsync(new MoveHeadRequest() { SpeedRadPerSec = speed }, cancellationToken: cancellationToken);
		}

		public async Task MoveLiftAsync(float speed, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.MoveLiftAsync(new MoveLiftRequest() { SpeedRadPerSec = speed }, cancellationToken: cancellationToken);
		}

		public async Task MoveHeadToAsync(float angle, float duration = 1f, float accel = 1f, float maxSpeed = 1f, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.SetHeadAngleAsync(new SetHeadAngleRequest() { AngleRad = angle, DurationSec = duration, AccelRadPerSec2 = accel, MaxSpeedRadPerSec = maxSpeed, IdTag = GetActionTagID() }, cancellationToken: cancellationToken);
		}

		public async Task MoveLiftToAsync(float height, float duration = 1f, float accel = 1f, float maxSpeed = 1f, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.SetLiftHeightAsync(new SetLiftHeightRequest() { HeightMm = height, DurationSec = duration, AccelRadPerSec2 = accel, MaxSpeedRadPerSec = maxSpeed, IdTag = GetActionTagID() }, cancellationToken: cancellationToken);
		}

		public async Task GoToPoseAsync(float x, float y, float angle, MotionSettings motionSettings = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var motion = Map<PathMotionProfile>(motionSettings ?? DefaultMotionSettings);
			var result = await Client.GoToPoseAsync(new GoToPoseRequest() { IdTag = GetActionTagID(), XMm = x, YMm = y, Rad = angle, MotionProf = motion }, cancellationToken: cancellationToken);
		}
	}
}
