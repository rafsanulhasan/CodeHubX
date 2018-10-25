using CodeHubX.Helpers;
using CodeHubX.Services;
using CodeHubX.Services.Hilite_me;
using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.Services;
using CodeHubX.UWP.Views;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.QueryStringDotNET;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamlBrewer.Uwp.Controls;
using static CodeHubX.UWP.Helpers.GlobalHelper;
using Issue = Octokit.Issue;
using PullRequest = Octokit.PullRequest;
using Repository = Octokit.Repository;

namespace CodeHubX.UWP
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application
	{
		private ExtendedExecutionSession _ExExecSession;
		private BackgroundTaskDeferral _AppTriggerDeferral;
		private BackgroundTaskDeferral _SyncDeferral;
		private BackgroundTaskDeferral _ToastActionDeferral;

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

		private void Application_UnhandledException(
		    object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			ToastHelper.ShowMessage(e.Message, e.Exception.StackTrace);
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
						await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), uri.Segments[1] + uri.Segments[2]);
						break;

					case "user":
						await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), uri.Segments[1]);
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
						(Window.Current.Content as Page).OpenFromSplashScreen(launchArgs.SplashScreen.ImageLocation);
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
					var mainPageType = typeof(FeedView);
					var backPageType = typeof(NotificationsView);
					if (Window.Current.Content == null)
					{
						Window.Current.Content = new MainPage(args);
					}
					else
					{
						var svc = SimpleIoc
						  .Default
						  .GetInstance<IAsyncNavigationService>();
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
									await svc.NavigateAsync(typeof(IssueDetailView), new Tuple<Repository, Issue>(repo, issue), backPageType: backPageType);

									break;

								case "showPr":
									var prNumber = int.Parse(toastArgs["prNumber"]);
									var pr = await PullRequestUtility.GetPullRequest(repoId, prNumber);
									tag += $"+P{pr.Number}";
									group = "PullRequests";
									await svc.NavigateAsync(typeof(PullRequestDetailView), new Tuple<Repository, PullRequest>(repo, pr), backPageType: backPageType);

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
							await svc.NavigateAsync(mainPageType);
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
				(Window.Current.Content as Frame).Navigate(typeof(NotificationsView));
				}
			}

			var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
			if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
			    backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
			{
				BackgroundTaskRegistrationService.GetGroup().BackgroundActivated += BackgroundActivityService.Start;
			}
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
			=> ToastHelper.ShowMessage($"Failed to load Page {e.SourcePageType.FullName}", e.Exception.ToString());

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

			deferral.Complete();
		}

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			InitializeComponent();

			Suspending += OnSuspending;
			UnhandledException += Application_UnhandledException;
			// Theme setup
			RequestedTheme = SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled) ? ApplicationTheme.Light : ApplicationTheme.Dark;
			SettingsService.Save(SettingsKeys.HighlightStyleIndex, (int) SyntaxHighlightStyleEnum.Monokai, false);
			SettingsService.Save(SettingsKeys.ShowLineNumbers, true, false);
			SettingsService.Save(SettingsKeys.LoadCommitsInfo, false, false);
			SettingsService.Save(SettingsKeys.IsAdsEnabled, false, false);
			SettingsService.Save(SettingsKeys.IsNotificationCheckEnabled, true, false);
			SettingsService.Save(SettingsKeys.HasUserDonated, false, false);

			AppCenter.Start("ecd96e4c-b301-48f3-b640-166a040f1d86", typeof(Analytics), typeof(Crashes));
		}

		protected override void OnActivated(IActivatedEventArgs args)
		{
			base.OnActivated(args);
			OnLaunchedOrActivated(args);
		}

		protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
		{
			try
			{
				BackgroundActivityService.Start(BackgroundTaskRegistrationService.GetGroup(), args);
			}
			catch
			{
				new BackgroundActivityService().Run(args);
			}
		}

		/// <summary>
		/// Invoked when the application is  launched normally by the end user. Other entry points will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the         launch request and process.
		/// </param>
		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
			base.OnLaunched(e);
			OnLaunchedOrActivated(e);
		}
	}
}