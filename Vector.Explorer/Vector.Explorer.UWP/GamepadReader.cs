using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vector.Explorer.ViewModel;
using Input = Windows.Gaming.Input;

namespace Vector.Explorer.UWP.ViewModel
{
	public class GamepadReader : IGamepad
	{
		public GamepadValues? ReadValues()
		{
			var gamepad = Input.Gamepad.Gamepads.FirstOrDefault();
			if (gamepad != null)
			{
				var result = gamepad.GetCurrentReading();
				return new GamepadValues()
				{
					LeftTrigger = result.LeftTrigger,
					RightTrigger = result.RightTrigger,
					LeftThumbstickX = result.LeftThumbstickX,
					LeftThumbstickY = result.LeftThumbstickY,
					RightThumbstickX = result.RightThumbstickX,
					RightThumbstickY = result.RightThumbstickY,
					Menu = result.Buttons.HasFlag(Input.GamepadButtons.Menu),
					View = result.Buttons.HasFlag(Input.GamepadButtons.View),
					A = result.Buttons.HasFlag(Input.GamepadButtons.A),
					B = result.Buttons.HasFlag(Input.GamepadButtons.B),
					X = result.Buttons.HasFlag(Input.GamepadButtons.X),
					Y = result.Buttons.HasFlag(Input.GamepadButtons.Y),
					DPadUp = result.Buttons.HasFlag(Input.GamepadButtons.DPadUp),
					DPadDown = result.Buttons.HasFlag(Input.GamepadButtons.DPadDown),
					DPadLeft = result.Buttons.HasFlag(Input.GamepadButtons.DPadLeft),
					DPadRight = result.Buttons.HasFlag(Input.GamepadButtons.DPadRight),
					LeftShoulder = result.Buttons.HasFlag(Input.GamepadButtons.LeftShoulder),
					RightShoulder = result.Buttons.HasFlag(Input.GamepadButtons.RightShoulder),
					LeftThumbstick = result.Buttons.HasFlag(Input.GamepadButtons.LeftThumbstick),
					RightThumbstick = result.Buttons.HasFlag(Input.GamepadButtons.RightThumbstick),
					Paddle1 = result.Buttons.HasFlag(Input.GamepadButtons.Paddle1),
					Paddle2 = result.Buttons.HasFlag(Input.GamepadButtons.Paddle2),
					Paddle3 = result.Buttons.HasFlag(Input.GamepadButtons.Paddle3),
					Paddle4 = result.Buttons.HasFlag(Input.GamepadButtons.Paddle4)
				};
			}
			return null;
		}
	}
}
