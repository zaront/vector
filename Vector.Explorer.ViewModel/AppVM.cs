using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

namespace Vector.Explorer.ViewModel
{
	public class AppVM : ViewModelBase
	{
		IRobotConnectionInfoStorage _robotConnection;
		INavigationService _navigation;
		IDialogService _dialog;
		ISettingsService _settings;

		public GamepadVM Gamepad { get; }
		public IList<RobotVM> Robots { get; }
		public ConnectionVM Connection { get; }

		public ICommand NewConnectionCommand { get; }

		public AppVM(IGamepad gamepad, INavigationService navigation, IDialogService dialog, ISettingsService settings)
		{
			//set fields
			Gamepad = new GamepadVM(gamepad);
			_navigation = navigation;
			_dialog = dialog;
			_settings = settings;
			Connection = new ConnectionVM(_navigation, _dialog, _settings, NewConnectionCreated);
			Robots = new ObservableCollection<RobotVM>();
			_robotConnection = new SettingsRobotConnection(_settings);
			NewConnectionCommand = new RelayCommand(NewConnection);

			//load previous robots
			var robotList = _settings.Get<string[]>("RobotList");
			if (robotList != null)
			{
				foreach (var robotName in robotList)
				{
					var conn = _robotConnection.Get(robotName);
					if (conn != null)
						AddRobot(conn);
				}
			}

			void NewConnection()
			{
				_navigation.NavigateTo(Pages.NewConnection.ToString());
			}

			void NewConnectionCreated(RobotConnectionInfo conn)
			{
				//add a new robot
				AddRobot(conn);

				//save connection info
				_robotConnection.Save(conn);

				//update robot list
				settings.Set("RobotList", Robots.Select(i => i.Connection.RobotName).ToArray());
			}

			void AddRobot(RobotConnectionInfo conn)
			{
				Robots.Add(new RobotVM(new Robot(_robotConnection), conn, _navigation, _dialog, _settings));
			}
		}
	}
}
