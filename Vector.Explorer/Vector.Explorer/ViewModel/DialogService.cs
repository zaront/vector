using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Vector.Explorer.ViewModel
{
	public class DialogService : IDialogService
	{
		Page _page;

		public DialogService(Page page)
		{
			//set fields
			_page = page;
		}

		public async Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
		{
			await _page.DisplayAlert(title, message, buttonText);
			afterHideCallback?.Invoke();
		}

		public async Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
		{
			await _page.DisplayAlert(title, error.Message, buttonText);
			afterHideCallback?.Invoke();
		}

		public async Task ShowMessage(string message, string title)
		{
			await _page.DisplayAlert(title, message, "OK");
		}

		public async Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
		{
			await _page.DisplayAlert(title, message, buttonText);
			afterHideCallback?.Invoke();
		}

		public async Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback)
		{
			var answer = await _page.DisplayActionSheet(message, buttonCancelText, buttonConfirmText);
			var result = answer == buttonConfirmText;
			afterHideCallback?.Invoke(result);
			return result;
		}

		public async Task ShowMessageBox(string message, string title)
		{
			await _page.DisplayAlert(title, message, "OK");
		}
	}
}
