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
		protected INavigationService Navigation;
		protected IDialogService Dialog;
		protected ISettingsService Settings;
		string _robotName;
		string _ipAddress;
		string _serialNum;
		string _userName;
		string _password;

		public event Action<RobotConnectionInfo> NewConnectionCreated;

		public RelayCommand ConnectCommand { get; }
		public ICommand CancelCommand { get; }

		public string RobotName { get => _robotName; set { Set(ref _robotName, value); ValidateConnect(); } }
		public string IpAddress { get => _ipAddress; set { Set(ref _ipAddress, value); ValidateConnect(); } }
		public string SerialNum { get => _serialNum; set { Set(ref _serialNum, value); ValidateConnect(); } }
		public string UserName { get => _userName; set { Set(ref _userName, value); ValidateConnect(); } }
		public string Password { get => _password; set { Set(ref _password, value); ValidateConnect(); } }

		public ConnectionVM(INavigationService navigation, IDialogService dialog, ISettingsService settings)
		{
			//set fields
			Navigation = navigation;
			Dialog = dialog;
			Settings = settings;
			ConnectCommand = new RelayCommandAsync(Connect) { Enabled = true };
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
				RobotName = "Vector-G7K2"; IpAddress = "192.168.1.189"; SerialNum = "00506620"; UserName = "zaron.thompson@gmail.com"; Password = "VectorRocks1!";
				var conn = await ApiAccess.GrantAsync(RobotName, IpAddress, SerialNum, UserName, Password);
				NewConnectionCreated?.Invoke(conn);
				Navigation.GoBack();
			}
			catch (MissingConnectionException)
			{
				if (await Dialog.ShowMessage("do cool stuff", "Authorize Robot", "OK", "Cancel", null))
				{
				}
			}
		}

		void Cancel()
		{
			Navigation.GoBack();
		}
	}
}
