using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;

namespace Vector.Explorer.ViewModel
{
    public class VectorControlVM : ViewModelBase
	{
		internal Robot Robot;
		internal INavigationService Navigation;
		internal IDialogService Dialog;

		public IGamepad Gamepad { get; }
		public ConnectionVM Connection { get; }

		public VectorControlVM(Robot robot, IGamepad gamepad, INavigationService Navigation, IDialogService dialog)
		{
			//set fields
			Robot = robot;
			Gamepad = gamepad;
			this.Navigation = Navigation;
			Dialog = dialog;
			Connection = new ConnectionVM(this);
			
		}
    }
}
