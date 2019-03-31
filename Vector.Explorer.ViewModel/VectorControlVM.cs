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
		internal ISettingsService Settings;

		public IGamepad Gamepad { get; }
		public ConnectionVM Connection { get; }

		public ICommand NewConnectionCommand { get; }

		public VectorControlVM(Robot robot, IGamepad gamepad, INavigationService navigation, IDialogService dialog, ISettingsService settings)
		{
			//set fields
			Robot = robot;
			Gamepad = gamepad;
			Navigation = navigation;
			Dialog = dialog;
			Settings = settings;
			Connection = new ConnectionVM(this);
			NewConnectionCommand = new RelayCommand(NewConnection);
		}

		void NewConnection()
		{
			Navigation.NavigateTo(Pages.GrantApiAccess.ToString());
		}

	}
}
