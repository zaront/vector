using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vector.Explorer.ViewModel
{
	public interface IGamepad
	{
		GamepadValues? ReadValues();
	}

	public struct GamepadValues
	{
		public double LeftTrigger;
		public double RightTrigger;
		public double LeftThumbstickX;
		public double LeftThumbstickY;
		public double RightThumbstickX;
		public double RightThumbstickY;
		public bool Menu;
		public bool View;
		public bool A;
		public bool B;
		public bool X;
		public bool Y;
		public bool DPadUp;
		public bool DPadDown;
		public bool DPadLeft;
		public bool DPadRight;
		public bool LeftShoulder;
		public bool RightShoulder;
		public bool LeftThumbstick;
		public bool RightThumbstick;
		public bool Paddle1;
		public bool Paddle2;
		public bool Paddle3;
		public bool Paddle4;
	}
}
