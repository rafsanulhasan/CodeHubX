using CodeHubX.Helpers;
using CodeHubX.Services;
using CodeHubX.Services.Hilite_me;
using CodeHubX.Views;
using Microsoft.QueryStringDotNET;
using Octokit;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamlBrewer.Uwp.Controls;
using static CodeHubX.Helpers.GlobalHelper;
using Exception = System.Exception;
using MainPage = CodeHubX.UWP.Views.MainPage;

namespace CodeHubX.UWP
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Windows.UI.Xaml.Application
	{
		private void Activate()
		{
			if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
			{
				var view = ApplicationView.GetForCurrentView();
				view.SetPreferredMinSize(new Size(width: 800, height: 600));

				var titleBar = ApplicationView.GetForCurrentView().TitleBar;
				if (titleBar != null)
				{
					titleBar.ButtonBackgroundColor = titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

					titleBar.ForegroundColor =
					titleBar.ButtonInactiveForegroundColor =
					titleBar.ButtonForegroundColor =
					Colors.White;
				}
			}


		}

		private async Task HandleProtocolActivationArguments(IActivatedEventArgs args)
		{
			if (!StringHelper.IsNullOrEmptyOrWhiteSpace(UserLogin))
			{
				var eventArgs = args as ProtocolActivatedEventArgs;
				var uri = eventArgs.Uri;
				var host = uri.Host;
				switch (host.ToLower())
				{
					case "repository":
						//await NavigationService.NavigateAsync(RepoDetailPage, uri.Segments[1] + uri.Segments[2]);
						break;

					case "user":
						//await NavigationService.NavigateAsync(DeveloperProfilePage, uri.Segments[1]);
						break;

				}
			}
		}

		private async void OnLaunchedOrActivated(IActivatedEventArgs args)
		{
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached)
			{
				DebugSettings.EnableFrameRateCounter = true;
			}

#endif
			// Set the right theme-depending color for the alternating rows
			if (SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled))
			{
				XAMLHelper.AssignValueToXAMLResource("OddAlternatingRowsBrush", new SolidColorBrush { Color = Color.FromArgb(0x08, 0, 0, 0) });
			}
			if (args is LaunchActivatedEventArgs launchArgs)
			{
				if (!launchArgs.PrelaunchActivated)
				{
					if (Window.Current.Content == null)
					{
						Window.Current.Content = new MainPage(null);
						(Window.Current.Content as Xamarin.Forms.Platform.UWP.WindowsPage).OpenFromSplashScreen(launchArgs.SplashScreen.ImageLocation);
					}
				}
				Activate();
				Window.Current.Activate();
			}
			else if (args is ToastNotificationActivatedEventArgs toastActivatedEventArgs)
			{
				if (args.Kind == ActivationKind.Protocol)
				{
					if (args.PreviousExecutionState == ApplicationExecutionState.Running)
					{
						await HandleProtocolActivationArguments(args);
					}
					else
					{
						if (Window.Current.Content == null)
						{
							Window.Current.Content = new MainPage(args);
						}
						Activate();
					}
				}
				else if (args.Kind == ActivationKind.ToastNotification)
				{
					var mainPage = new FeedsPage();
					//var backPageType = NotificationsPage;
					if (Window.Current.Content == null)
					{
						Window.Current.Content = new MainPage(args);
					}
					else
					{
						try
						{
							var toastArgs = QueryString.Parse(toastActivatedEventArgs.Argument);
							var notificationId = toastArgs["notificationId"] as string;
							var repoId = long.Parse(toastArgs["repoId"]);

							string group = null,
								  tag = $"N{notificationId}+R{repoId}";

							var repo = await RepositoryUtility.GetRepository(repoId);

							switch (toastArgs["action"])
							{
								case "showIssue":
									var issueNumber = int.Parse(toastArgs["issueNumber"]);

									var issue = await IssueUtility.GetIssue(repo.Id, issueNumber);
									tag += $"+I{issueNumber}";
									group = "Issues";
									//await NavigationService.NavigateAsync(IssueDetailPage, new Tuple<Repository, Issue>(repo, issue), backPageType: backPageType);

									break;

								case "showPr":
									var prNumber = int.Parse(toastArgs["prNumber"]);
									var pr = await PullRequestUtility.GetPullRequest(repoId, prNumber);
									tag += $"+P{pr.Number}";
									group = "PullRequests";
									//await NavigationService.NavigateAsync(PullRequestDetailPage, new Tuple<Repository, PullRequest>(repo, pr), backPageType: backPageType);

									break;
							}
							if (!StringHelper.IsNullOrEmptyOrWhiteSpace(tag) && !StringHelper.IsNullOrEmptyOrWhiteSpace(group))
							{
								ToastNotificationManager.History.Remove(tag, group);
							}
							if (!StringHelper.IsNullOrEmptyOrWhiteSpace(notificationId))
							{
								await NotificationsService.MarkNotificationAsRead(notificationId);
							}
						}
						catch
						{
							//await NavigationService.NavigateAsync(mainPage);
						}
					}

					Activate();
					Window.Current.Activate();
				}
			}
			else if (args is StartupTaskActivatedEventArgs startupTaskActivatedEventArgs)
			{
				if (args.Kind == ActivationKind.StartupTask)
				{
					var payload = ActivationKind.StartupTask.ToString();
					if (Window.Current.Content == null)
					{
						Window.Current.Content = new MainPage(args);
					}
					//(Window.Current.Content as Frame).Navigate(typeof(NotificationsView));
				}
			}

			var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
			if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
			    backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
			{
				//BackgroundTaskRegistrationService.GetGroup().BackgroundActivated += BackgroundActivityService.Start;
			}
		}

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			InitializeComponent();
			SettingsService.Load = () => ApplicationData.Current.LocalSettings.Values;
			Suspending += OnSuspending;

			RequestedTheme = SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled) ? ApplicationTheme.Light : ApplicationTheme.Dark;
			SettingsService.Save(SettingsKeys.HighlightStyleIndex, (int) SyntaxHighlightStyleEnum.Monokai, false);
			SettingsService.Save(SettingsKeys.ShowLineNumbers, true, false);
			SettingsService.Save(SettingsKeys.LoadCommitsInfo, false, false);
			SettingsService.Save(SettingsKeys.IsAdsEnabled, false, false);
			SettingsService.Save(SettingsKeys.IsNotificationCheckEnabled, true, false);
			SettingsService.Save(SettingsKeys.HasUserDonated, false, false);
		}

		protected override void OnActivated(IActivatedEventArgs args)
		{
			base.OnActivated(args);
			OnLaunchedOrActivated(args);
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (!(Window.Current.Content is Frame rootFrame))
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				Xamarin.Forms.Forms.Init(e);

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				Window.Current.Content = rootFrame;
			}

			if (rootFrame.Content == null)
			{
				// When the navigation stack isn't restored navigate to the first page,
				// configuring the new page by passing required information as a navigation
				// parameter
				rootFrame.Navigate(typeof(MainPage), e.Arguments);
			}
			// Ensure the current window is active
			Window.Current.Activate();
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		private void OnNavigationFailed(object sender, NavigationFailedEventArgs e) => throw new Exception("Failed to load Page " + e.SourcePageType.FullName);

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			//TODO: Save application state and stop any background activity
			deferral.Complete();
		}
	}
}