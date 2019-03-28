using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vector.Explorer.ViewModel
{
	public interface IGamepad : INotifyPropertyChanged
	{
		Task BeginPolling(int interval = 100, CancellationToken cancellationToken = default(CancellationToken));
		void Poll();
		double LeftTrigger { get; set; }
		double RightTrigger { get; set; }
		double LeftThumbstickX { get; set; }
		double LeftThumbstickY { get; set; }
		double RightThumbstickX { get; set; }
		double RightThumbstickY { get; set; }
		bool Menu { get; set; }
		bool View { get; set; }
		bool A { get; set; }
		bool B { get; set; }
		bool X { get; set; }
		bool Y { get; set; }
		bool DPadUp { get; set; }
		bool DPadDown { get; set; }
		bool DPadLeft { get; set; }
		bool DPadRight { get; set; }
		bool LeftShoulder { get; set; }
		bool RightShoulder { get; set; }
		bool LeftThumbstick { get; set; }
		bool RightThumbstick { get; set; }
		bool Paddle1 { get; set; }
		bool Paddle2 { get; set; }
		bool Paddle3 { get; set; }
		bool Paddle4 { get; set; }
	}
}
