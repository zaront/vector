using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vector.Explorer.ViewModel
{
	public static class WireupExtention
	{
		public static void WireupViewModel(this App app, IGamepad gamepad = null)
		{
			var nav = new NavigationService(app.MainPage.Navigation);
			var dialog = new DialogService(app.MainPage);
			var settings = new SettingsService();
			var settingsRobotConnection = new SettingsRobotConnection(settings);
			var robot = new Robot(settingsRobotConnection);
			var managerVM = new RobotManagerVM(gamepad, nav, dialog, settings);
			var g = managerVM.Gamepad.BeginPolling();

			//databind
			app.BindingContext = managerVM;
		}
	}
}
