using CodeHub.ViewModels.Settings;
using GalaSoft.MvvmLight.Messaging;
using System;
using Windows.UI.Notifications.Management;
using Windows.UI.Xaml;
using static CodeHub.Helpers.GlobalHelper;

namespace CodeHub.Views.Settings
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class NotificationSettingsView : SettingsDetailPageBase
	{
		private NofiticationSettingsViewModel ViewModel;

		public NotificationSettingsView()
		{
			InitializeComponent();

			ViewModel = new NofiticationSettingsViewModel();
			DataContext = ViewModel;

			Messenger.Default.Register<OnOffMessage>(this, m =>
			{
				Bindings.Update();
			});

			ViewModel.PropertyChanged += ViewModel_PropertyChanged;
		}

		private async void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ViewModel.IsToastEnabled))
			{
				if (ViewModel.IsToastEnabled)
				{
					// Get the listener
					var listener = UserNotificationListener.Current;

					// And request access to the user's notifications (must be called from UI thread)
					var accessStatus = await listener.RequestAccessAsync();

					switch (accessStatus)
					{
						// This means the user has granted access.
						case UserNotificationListenerAccessStatus.Allowed:

							// Yay! Proceed as normal
							break;

						// This means the user has denied access.
						// Any further calls to RequestAccessAsync will instantly
						// return Denied. The user must go to the Windows settings
						// and manually allow access.
						case UserNotificationListenerAccessStatus.Denied:

							// Show UI explaining that listener features will not
							// work until user allows access.
							ViewModel.IsToastEnabled = false;
							break;

						// This means the user closed the prompt without
						// selecting either allow or deny. Further calls to
						// RequestAccessAsync will show the dialog again.
						case UserNotificationListenerAccessStatus.Unspecified:

							// Show UI that allows the user to bring up the prompt again
							ViewModel.IsToastEnabled = false;
							break;
					}

					Messenger.Default.Send(new OnOffMessage { InOn = ViewModel.IsToastEnabled });
				}
			}
			else if (e.PropertyName == nameof(ViewModel.IsLiveTilesEnabled))
				Messenger.Default.Send(new OnOffMessage { InOn = ViewModel.IsLiveTilesEnabled });
			else if (e.PropertyName == nameof(ViewModel.IsLiveTilesBadgeEnabled))
				Messenger.Default.Send(new OnOffMessage { InOn = ViewModel.IsLiveTilesBadgeEnabled });

		}

		private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
		{
			if (e.NewState != null)
				TryNavigateBackForDesktopState(e.NewState.Name);
		}
	}
}
