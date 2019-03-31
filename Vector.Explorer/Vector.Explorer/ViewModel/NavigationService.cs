using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xamarin.Forms;

namespace Vector.Explorer.ViewModel
{
	public class NavigationService : INavigationService
	{
		INavigation _nav;

		static Dictionary<string, Type> _pages;

		public NavigationService(INavigation nav)
		{
			//set fields
			_nav = nav;

			if (_pages == null)
				_pages = GetPages();
		}

		Dictionary<string, Type> GetPages()
		{
			return new Dictionary<string, Type>()
			{
				//register pages
				{Pages.NewConnection.ToString(), typeof(NewConnection) }
			};
		}

		public string CurrentPageKey
		{
			get
			{
				var page = _nav.NavigationStack.LastOrDefault();
				if (page == null)
					return null;
				var pageKey = _pages.FirstOrDefault(i => i.Value == page.GetType());
				if (pageKey.Value == null)
					return null;
				return pageKey.Key;
			}
		}

		public void GoBack()
		{
			_nav.PopModalAsync();
		}

		public void NavigateTo(string pageKey)
		{
			//get page
			if (!_pages.ContainsKey(pageKey))
				return;
			var pageType = _pages[pageKey];

			//create instance
			var page = Activator.CreateInstance(pageType) as Page;

			//navigate
			_nav.PushModalAsync(page);
		}

		public void NavigateTo(string pageKey, object parameter)
		{
			//get page
			if (!_pages.ContainsKey(pageKey))
				return;
			var pageType = _pages[pageKey];

			//create instance
			var page = Activator.CreateInstance(pageType, parameter) as Page;

			//navigate
			_nav.PushModalAsync(page);
		}
	}
}
