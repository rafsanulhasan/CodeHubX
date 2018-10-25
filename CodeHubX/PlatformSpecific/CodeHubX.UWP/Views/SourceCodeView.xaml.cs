﻿using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using Windows.UI.Xaml.Navigation;

namespace CodeHubX.UWP.Views
{

	public sealed partial class SourceCodeView : Windows.UI.Xaml.Controls.Page
	{
		public SourceCodeViewmodel ViewModel;

		private void TopScroller_OnTopScrollingRequested(object sender, EventArgs e) 
			=> ContentListView.ScrollToTheTop();

		public SourceCodeView()
		{
			Loaded += (s, e) => TopScroller.InitializeScrollViewer(ContentListView);
			InitializeComponent();
			ViewModel = new SourceCodeViewmodel();
			DataContext = ViewModel;
			Unloaded += (s, e) => TopScroller.Dispose();
			NavigationCacheMode = NavigationCacheMode.Required;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			if (e.NavigationMode == NavigationMode.Back)
			{
				Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = (e.Parameter as Repository).FullName });
				ContentListView.SelectedIndex = -1;
				return;
			}
			if (e.Parameter as Repository != ViewModel.Repository && ViewModel.Content != null)
			{
				ViewModel.Content.Clear();
			}
			await ViewModel.Load(e.Parameter as Repository);
		}
	}
}
