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
		Robot _robot;
		INavigationService _nav;
		IDialogService _dialog;

		public ICommand ConnectCommand { get; }

		public IGamepad Gamepad { get; }

		public VectorControlVM(Robot robot, IGamepad gamepad, INavigationService Navigation, IDialogService dialog)
		{
			//set fields
			_robot = robot;
			Gamepad = gamepad;
			_nav = Navigation;
			_dialog = dialog;
			ConnectCommand = new RelayCommandAsync<string>(Connect) { DisplayName = _robot.IsConnected ? "Disconnect" : "Connect" };
		}

		public async Task Connect(string robotName)
		{
			try
			{
				await _robot.ConnectAsync(robotName);
			}
			catch (MissingConnectionException)
			{
				if (await _dialog.ShowMessage("do cool stuff", "Authorize Robot", "OK", "Cancel", null))
					_nav.NavigateTo(Pages.GrantApiAccess.ToString());
			}
		}
    }
}
