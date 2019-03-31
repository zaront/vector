using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Vector.Explorer.ViewModel
{
	public class ConnectionVM : ViewModelBase
	{
		VectorControlVM _vectorVM;
		string _robotName;
		string _ipAddress;
		string _serialNum;
		string _userName;
		string _password;
		RobotConnectionStorage _robotConnectionStorage;

		public RelayCommand ConnectCommand { get; }

		public string RobotName { get => _robotName; set { Set(ref _robotName, value); ValidateConnect(); } }
		public string IpAddress { get => _ipAddress; set { Set(ref _ipAddress, value); ValidateConnect(); } }
		public string SerialNum { get => _serialNum; set { Set(ref _serialNum, value); ValidateConnect(); } }
		public string UserName { get => _userName; set { Set(ref _userName, value); ValidateConnect(); } }
		public string Password { get => _password; set { Set(ref _password, value); ValidateConnect(); } }

		public ConnectionVM(VectorControlVM vectorVM)
		{
			//set fields
			_vectorVM = vectorVM;
			_robotConnectionStorage = new RobotConnectionStorage(_vectorVM.Settings);
			ConnectCommand = new RelayCommandAsync(Connect) { DisplayName = GetConnectAction(), Enabled = false };
		}

		string GetConnectAction()
		{
			return _vectorVM.Robot.IsConnected ? "Disconnect" : "Connect";
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
			//try
			//{
			//	await Robot.ConnectAsync(robotName);
			//}
			//catch (MissingConnectionException)
			//{
			//	if (await Dialog.ShowMessage("do cool stuff", "Authorize Robot", "OK", "Cancel", null))
			//		Navigation.NavigateTo(Pages.GrantApiAccess.ToString());
			//}
		}

		public async Task GrantApiAccess()
		{
			//await Robot.GrantApiAccessAsync()
		}


		class RobotConnectionStorage : IRobotConnectionInfoStorage
		{
			ISettingsService _settings;

			public RobotConnectionStorage(ISettingsService settings)
			{
				//set fields
				_settings = settings;
			}

			public RobotConnectionInfo Get(string robotName)
			{
				return _settings.Get<RobotConnectionInfo>(GetKey(robotName));
			}

			public void Remove(string robotName)
			{
				_settings.Remove(GetKey(robotName));
			}

			public void Save(RobotConnectionInfo connection)
			{
				_settings.Set(GetKey(connection.RobotName), connection);
			}

			string GetKey(string robotName)
			{
				return $"{robotName}_RobotConnectionInfo";
			}
		}
	}
}
