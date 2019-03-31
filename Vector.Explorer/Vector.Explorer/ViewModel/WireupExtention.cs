using System;
using System.Collections.Generic;
using System.Text;

namespace Vector.Explorer.ViewModel
{
    public static class WireupExtention
    {
		public static void WireupViewModel(this App app, IGamepad gamepad = null)
		{
			//wire up
			var nav = new NavigationService(app.MainPage.Navigation);
			var dialog = new DialogService(app.MainPage);
			var settings = new SettingsService();
			var settingsRobotConnection = new SettingsRobotConnection(settings);
			var robot = new Robot(settingsRobotConnection);
			var appVM = new AppVM(gamepad, nav, dialog, settings);
			var g = appVM.Gamepad.BeginPolling();

			//databind
			app.BindingContext = appVM;
		}
    }
}
