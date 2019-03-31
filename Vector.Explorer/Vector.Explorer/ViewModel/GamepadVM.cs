using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vector.Explorer.ViewModel
{
	public class GamepadVM : ViewModelBase
	{
		IGamepad _gamepad;

		double _leftTrigger;
		double _rightTrigger;
		double _leftThumbstickX;
		double _leftThumbstickY;
		double _rightThumbstickX;
		double _rightThumbstickY;
		bool _menu;
		bool _view;
		bool _a;
		bool _b;
		bool _x;
		bool _y;
		bool _dPadUp;
		bool _dPadDown;
		bool _dPadLeft;
		bool _dPadRight;
		bool _leftShoulder;
		bool _rightShoulder;
		bool _leftThumbstick;
		bool _rightThumbstick;
		bool _paddle1;
		bool _paddle2;
		bool _paddle3;
		bool _paddle4;

		public GamepadVM(IGamepad gamepad)
		{
			//set fields
			_gamepad = gamepad;
		}

		public async Task BeginPolling(int interval = 20, CancellationToken cancellationToken = default(CancellationToken))
		{
			//validate
			if (_gamepad == null)
				return;

			while(!cancellationToken.IsCancellationRequested)
			{
				Poll();
				await Task.Delay(interval);
			}
		}

		public void Poll()
		{
			var gamepadValues = _gamepad?.ReadValues();
			if (gamepadValues != null)
			{
				var result = gamepadValues.Value;
				LeftTrigger = result.LeftTrigger;
				RightTrigger = result.RightTrigger;
				LeftThumbstickX = result.LeftThumbstickX;
				LeftThumbstickY = result.LeftThumbstickY;
				RightThumbstickX = result.RightThumbstickX;
				RightThumbstickY = result.RightThumbstickY;
				Menu = result.Menu;
				View = result.View;
				A = result.A;
				B = result.B;
				X = result.X;
				Y = result.Y;
				DPadUp = result.DPadUp;
				DPadDown = result.DPadDown;
				DPadLeft = result.DPadLeft;
				DPadRight = result.DPadRight;
				LeftShoulder = result.LeftShoulder;
				RightShoulder = result.RightShoulder;
				LeftThumbstick = result.LeftThumbstick;
				RightThumbstick = result.RightThumbstick;
				Paddle1 = result.Paddle1;
				Paddle2 = result.Paddle2;
				Paddle3 = result.Paddle3;
				Paddle4 = result.Paddle4;
			}
		}

		public double LeftTrigger { get => _leftTrigger; set => Set(ref _leftTrigger, value); }
		public double RightTrigger { get => _rightTrigger; set => Set(ref _rightTrigger, value); }
		public double LeftThumbstickX { get => _leftThumbstickX; set => Set(ref _leftThumbstickX, value); }
		public double LeftThumbstickY { get => _leftThumbstickY; set => Set(ref _leftThumbstickY, value); }
		public double RightThumbstickX { get => _rightThumbstickX; set => Set(ref _rightThumbstickX, value); }
		public double RightThumbstickY { get => _rightThumbstickY; set => Set(ref _rightThumbstickY, value); }
		public bool Menu { get => _menu; set => Set(ref _menu, value); }
		public bool View { get => _view; set => Set(ref _view, value); }
		public bool A { get => _a; set => Set(ref _a, value); }
		public bool B { get => _b; set => Set(ref _b, value); }
		public bool X { get => _x; set => Set(ref _x, value); }
		public bool Y { get => _y; set => Set(ref _y, value); }
		public bool DPadUp { get => _dPadUp; set => Set(ref _dPadUp, value); }
		public bool DPadDown { get => _dPadDown; set => Set(ref _dPadDown, value); }
		public bool DPadLeft { get => _dPadLeft; set => Set(ref _dPadLeft, value); }
		public bool DPadRight { get => _dPadRight; set => Set(ref _dPadRight, value); }
		public bool LeftShoulder { get => _leftShoulder; set => Set(ref _leftShoulder, value); }
		public bool RightShoulder { get => _rightShoulder; set => Set(ref _rightShoulder, value); }
		public bool LeftThumbstick { get => _leftThumbstick; set => Set(ref _leftThumbstick, value); }
		public bool RightThumbstick { get => _rightThumbstick; set => Set(ref _rightThumbstick, value); }
		public bool Paddle1 { get => _paddle1; set => Set(ref _paddle1, value); }
		public bool Paddle2 { get => _paddle2; set => Set(ref _paddle2, value); }
		public bool Paddle3 { get => _paddle3; set => Set(ref _paddle3, value); }
		public bool Paddle4 { get => _paddle4; set => Set(ref _paddle4, value); }

	}
}
