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
		protected Robot Robot;
		protected INavigationService Navigation;
		protected IDialogService Dialog;
		protected ISettingsService Settings;

		public RobotConnectionInfo Connection { get; }
		public RelayCommand ConnectCommand { get; }

		public RobotVM(Robot robot, RobotConnectionInfo connection, INavigationService navigation, IDialogService dialog, ISettingsService settings)
		{
			//set fields
			Robot = robot;
			Connection = connection;
			Navigation = navigation;
			Dialog = dialog;
			Settings = settings;
			ConnectCommand = new RelayCommandAsync(Connect) { DisplayName = GetConnectAction() };
		}

		string GetConnectAction()
		{
			return Robot.IsConnected ? "Disconnect" : "Connect";
		}

		async Task Connect()
		{
			try
			{
				await Robot.ConnectAsync(Connection);
				ConnectCommand.DisplayName = GetConnectAction();
			}
			catch
			{
			}
		}

	}
}
