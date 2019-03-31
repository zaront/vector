using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Vector.Explorer.ViewModel
{
	public class RobotVM : ViewModelBase
	{
		Robot _robot;
		INavigationService _navigation;
		IDialogService _dialog;
		ISettingsService _settings;

		public RobotConnectionInfo Connection { get; }
		public RelayCommand ConnectCommand { get; }

		public RobotVM(Robot robot, RobotConnectionInfo connection, INavigationService navigation, IDialogService dialog, ISettingsService settings)
		{
			//set fields
			_robot = robot;
			Connection = connection;
			_navigation = navigation;
			_dialog = dialog;
			_settings = settings;
			ConnectCommand = new RelayCommandAsync(Connect) { DisplayName = GetConnectAction() };
		}

		string GetConnectAction()
		{
			return _robot.IsConnected ? "Disconnect" : "Connect";
		}

		async Task Connect()
		{
			try
			{
				await _robot.ConnectAsync(Connection);
				ConnectCommand.DisplayName = GetConnectAction();
			}
			catch
			{
			}
		}

	}
}
