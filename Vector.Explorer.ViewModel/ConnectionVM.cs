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
		//string _

		public ICommand ConnectCommand { get; }

		public ConnectionVM(VectorControlVM vectorVM)
		{
			//set fields
			_vectorVM = vectorVM;
			ConnectCommand = new RelayCommandAsync(Connect) { DisplayName = GetConnectAction() };
		}

		string GetConnectAction()
		{
			return _vectorVM.Robot.IsConnected ? "Disconnect" : "Connect";
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
	}
}
