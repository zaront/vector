using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Vector.Explorer.UWP.ViewModel;
using Vector.Explorer.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Vector.Explorer.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

			//wire up
			var app = new Vector.Explorer.App();
			var gamepad = new GamepadVM();
			var g = gamepad.BeginPolling();
			var nav = new NavigationService(app.MainPage.Navigation);
			var dialog = new DialogService(app.MainPage);
			var vector = new VectorControlVM(new Robot(), gamepad, nav, dialog);

			//databind
			app.BindingContext = vector;

			//start app
			LoadApplication(app);
        }
    }
}
