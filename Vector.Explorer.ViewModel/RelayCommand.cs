using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Vector.Explorer.ViewModel
{
	public class RelayCommand : ICommand, INotifyPropertyChanged
	{
		Action _execute;
		Action<object> _executeParameter;
		bool _canExecute = true;
		string _displayName;

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler CanExecuteChanged;

		protected RelayCommand() { }

		public RelayCommand(Action execute)
		{
			_execute = execute;
		}
		public RelayCommand(Action<object> execute)
		{
			_executeParameter = execute;
		}

		public bool Enabled
		{
			get { return _canExecute; }
			set
			{
				if (_canExecute == value)
					return;
				_canExecute = value;
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute;
		}

		public virtual void Execute(object parameter)
		{
			_execute?.Invoke();
			_executeParameter?.Invoke(parameter);
		}

		public string DisplayName
		{
			get { return _displayName; }
			set { _displayName = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisplayName")); }
		}
	}

	public class RelayCommand<T> : RelayCommand
	{
		Action<T> _executeParameterT;

		public RelayCommand(Action<T> execute)
		{
			_executeParameterT = execute;
		}

		public override void Execute(object parameter)
		{
			_executeParameterT?.Invoke((T)parameter);
		}
	}

	public class RelayCommandAsync : RelayCommand
	{
		Func<Task> _executeAsync;
		Func<object, Task> _executeParameterAsync;

		protected RelayCommandAsync() { }

		public RelayCommandAsync(Func<Task> execute)
		{
			_executeAsync = execute;
		}
		public RelayCommandAsync(Func<object, Task> execute)
		{
			_executeParameterAsync = execute;
		}

		public async override void Execute(object parameter)
		{
			if (_executeAsync != null)
				await _executeAsync();
			if (_executeParameterAsync != null)
				await _executeParameterAsync(parameter);
		}
	}

	public class RelayCommandAsync<T> : RelayCommandAsync
	{
		Func<T, Task> _executeParameterAsyncT;

		public RelayCommandAsync(Func<T, Task> execute)
		{
			_executeParameterAsyncT = execute;
		}

		public async override void Execute(object parameter)
		{
			if (_executeParameterAsyncT != null)
				await _executeParameterAsyncT((T)parameter);
		}
	}
}
