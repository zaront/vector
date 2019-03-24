using System;
using System.Collections.Generic;
using System.Text;

namespace Vector
{
	public class RobotState
	{
		public Pose Pose;
		public float PoseAngleRad;
		public float PosePitchRad;
		public float LeftWheelSpeedMmps;
		public float RightWheelSpeedMmps;
		public float HeadAngleRad;
		public float LiftHeightMM;
		public Vector3 Accel;
		public Vector3 Gyro;
		public int CarryingObjectID;
		public int CarryingObjectOnTopID;
		public int HeadTrackingObjectID;
		public int LocalizedToObjectID;
		public int LastImageTimeStamp;
		public int Status;
		public ProxData ProxData;
		public TouchData TouchData;
	}

	public struct Vector3
	{
		public float X;
		public float Y;
		public float Z;
	}

	public struct Pose
	{
		// Translation
		public float X;
		public float Y;
		public float Z;

		// Rotation quaternion
		public float Q0;
		public float Q1;
		public float Q2;
		public float Q3;

		public int OriginID; // Which coordinate frame this pose is in (0 for none or unknown)
	}

	public class ProxData
	{
		public int DistanceMM;
		public float SignalQuality;
		public bool IsInValidRange; // Distance is within valid range
		public bool IsValidSignalQuality; // Signal quality is sufficiently strong to trust that something was detected
		public bool IsLiftInFov; // Lift (or object on lift) is occluding the sensor
		public bool IsTooPitched; // Robot is too far pitched up or down
	}

	public class TouchData
	{
		public int RawTouchValue; // Raw input from the touch sensor
		public bool IsBeingTouched; // Robot's context aware evaluation of whether it currently is or isn't being touched
	}

	public enum RobotStatus
	{
		NONE = 0x0,
		IS_MOVING = 0x1,
		IS_CARRYING_BLOCK = 0x2,
		IS_PICKING_OR_PLACING = 0x4,
		IS_PICKED_UP = 0x8,
		IS_BUTTON_PRESSED = 0x10,
		IS_FALLING = 0x20,
		IS_ANIMATING = 0x40,
		IS_PATHING = 0x80,
		LIFT_IN_POS = 0x100,
		HEAD_IN_POS = 0x200,
		CALM_POWER_MODE = 0x400,
		IS_ON_CHARGER = 0x1000,
		IS_CHARGING = 0x2000,
		CLIFF_DETECTED = 0x4000,
		ARE_WHEELS_MOVING = 0x8000,
		IS_BEING_HELD = 0x10000,
		IS_MOTION_DETECTED = 0x20000
	}
}
