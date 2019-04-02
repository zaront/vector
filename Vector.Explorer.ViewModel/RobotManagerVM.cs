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
	public class RobotManagerVM : ViewModelBase
	{
		protected IRobotConnectionInfoStorage ConnectionStorage;
		protected INavigationService Navigation;
		protected IDialogService Dialog;
		protected ISettingsService Settings;

		public GamepadVM Gamepad { get; }
		public IList<RobotVM> Robots { get; }
		public ConnectionVM Connection { get; }

		public ICommand NewConnectionCommand { get; }

		public RobotManagerVM(IGamepad gamepad, INavigationService navigation, IDialogService dialog, ISettingsService settings)
		{
			//set fields
			Gamepad = new GamepadVM(gamepad);
			Navigation = navigation;
			Dialog = dialog;
			Settings = settings;
			Connection = CreateConnection();
			Connection.NewConnectionCreated += NewConnectionCreated;
			Robots = new ObservableCollection<RobotVM>();
			ConnectionStorage = new ConnectionStorage(Settings);
			NewConnectionCommand = new RelayCommand(NewConnection);

			//load previous robots
			var robotList = Settings.Get<string[]>("RobotList");
			if (robotList != null)
			{
				foreach (var robotName in robotList)
				{
					var conn = ConnectionStorage.Get(robotName);
					if (conn != null)
						AddRobot(conn);
				}
			}
		}

		void NewConnection()
		{
			Navigation.NavigateTo(Pages.NewConnection.ToString());
		}

		void NewConnectionCreated(RobotConnectionInfo conn)
		{
			//add a new robot
			AddRobot(conn);

			//save connection info
			ConnectionStorage.Save(conn);

			//update robot list
			Settings.Set("RobotList", Robots.Select(i => i.Connection.RobotName).ToArray());
		}
		
		void AddRobot(RobotConnectionInfo conn)
		{
			Robots.Add(CreateRobot(conn));
		}

		protected virtual RobotVM CreateRobot(RobotConnectionInfo conn)
		{
			return new RobotVM(new Robot(ConnectionStorage), conn, Navigation, Dialog, Settings);
		}

		protected virtual ConnectionVM CreateConnection()
		{
			return new ConnectionVM(Navigation, Dialog, Settings);
		}
	}
}
