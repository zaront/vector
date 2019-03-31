using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Vector.Explorer.ViewModel
{
	public class ConnectionVM : ViewModelBase
	{
		INavigationService _navigation;
		IDialogService _dialog;
		ISettingsService _settings;
		Action<RobotConnectionInfo> _newConnectionCreated;
		string _robotName;
		string _ipAddress;
		string _serialNum;
		string _userName;
		string _password;

		public RelayCommand ConnectCommand { get; }
		public ICommand CancelCommand { get; }

		public string RobotName { get => _robotName; set { Set(ref _robotName, value); ValidateConnect(); } }
		public string IpAddress { get => _ipAddress; set { Set(ref _ipAddress, value); ValidateConnect(); } }
		public string SerialNum { get => _serialNum; set { Set(ref _serialNum, value); ValidateConnect(); } }
		public string UserName { get => _userName; set { Set(ref _userName, value); ValidateConnect(); } }
		public string Password { get => _password; set { Set(ref _password, value); ValidateConnect(); } }

		public ConnectionVM(INavigationService navigation, IDialogService dialog, ISettingsService settings, Action<RobotConnectionInfo> newConnectionCreated)
		{
			//set fields
			_navigation = navigation;
			_dialog = dialog;
			_settings = settings;
			_newConnectionCreated = newConnectionCreated;
			ConnectCommand = new RelayCommandAsync(Connect) { Enabled = false };
			CancelCommand = new RelayCommand(Cancel);
		}

		void ValidateConnect()
		{
			ConnectCommand.Enabled = !string.IsNullOrWhiteSpace(RobotName) &&
				!string.IsNullOrWhiteSpace(IpAddress) &&
				!string.IsNullOrWhiteSpace(SerialNum) &&
				!string.IsNullOrWhiteSpace(UserName) &&
				!string.IsNullOrWhiteSpace(Password);
		}

		public async Task Connect()
		{
			try
			{
				var conn = await ApiAccess.GrantAsync(RobotName, IpAddress, SerialNum, UserName, Password);
				_newConnectionCreated(conn);
				_navigation.GoBack();
			}
			catch (MissingConnectionException)
			{
				if (await _dialog.ShowMessage("do cool stuff", "Authorize Robot", "OK", "Cancel", null))
				{
				}
			}
		}

		void Cancel()
		{
			_navigation.GoBack();
		}
	}
}
