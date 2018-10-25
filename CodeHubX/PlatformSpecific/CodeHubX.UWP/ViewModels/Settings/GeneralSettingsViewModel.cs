using CodeHubX.Helpers;
using CodeHubX.UWP.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Globalization;
using Windows.Globalization;

namespace CodeHubX.UWP.ViewModels.Settings
{
	public class GeneralSettingsViewModel : ObservableObject
	{
		private bool _LoadCommitsInfo = SettingsService.Get<bool>(SettingsKeys.LoadCommitsInfo);
		/// <summary>
		/// Gets or sets whether or not to load the additional commits info when browsing the contents of a repository
		/// </summary>
		public bool LoadCommitsInfo
		{
			get => _LoadCommitsInfo;
			set
			{
				if (_LoadCommitsInfo != value)
				{
					_LoadCommitsInfo = value;
					SettingsService.Save(SettingsKeys.LoadCommitsInfo, value);
					RaisePropertyChanged();
				}
			}
		}

		private bool _IsAdsEnabled = SettingsService.Get<bool>(SettingsKeys.IsAdsEnabled);
		/// <summary>
		/// Gets or sets whether or not the Ads are enabled in the app
		/// </summary>
		public bool IsAdsEnabled
		{
			get => _IsAdsEnabled;
			set
			{
				if (_IsAdsEnabled != value)
				{
					_IsAdsEnabled = value;
					SettingsService.Save(SettingsKeys.IsAdsEnabled, value);
					Messenger.Default.Send(new GlobalHelper.AdsEnabledMessageType());
					RaisePropertyChanged();
				}
			}
		}

		private bool _CanDisableAds = GlobalHelper.HasAlreadyDonated;
		public bool CanDisableAds
		{
			get => _CanDisableAds;
			set => Set(() => CanDisableAds, ref _CanDisableAds, value);
		}

		private bool _IsNotificationCheckEnabled = SettingsService.Get<bool>(SettingsKeys.IsNotificationCheckEnabled);
		/// <summary>
		/// Gets or sets whether API calls for unread notifications will be frequently made
		/// </summary>
		public bool IsNotificationCheckEnabled
		{
			get => _IsNotificationCheckEnabled;
			set
			{
				if (_IsNotificationCheckEnabled != value)
				{
					_IsNotificationCheckEnabled = value;
					SettingsService.Save(SettingsKeys.IsNotificationCheckEnabled, value);
					RaisePropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the list of app's supported languages
		/// </summary>
		public List<Language> AvailableUiLanguages { get; private set; } = new List<Language>();

		private int _selectedUiLanguageIndex;
		/// <summary>
		/// Gets or sets the index of the currently selected UI language
		/// </summary>
		public int SelectedUiLanguageIndex
		{
			get => _selectedUiLanguageIndex;
			set
			{
				Set(() => SelectedUiLanguageIndex, ref _selectedUiLanguageIndex, value);

				try
				{
					var language = AvailableUiLanguages[value];
					ApplicationLanguages.PrimaryLanguageOverride = language.LanguageTag;
				}
				catch { }
			}
		}

		public GeneralSettingsViewModel()
		{
			foreach (var languageTag in ApplicationLanguages.ManifestLanguages)
				AvailableUiLanguages.Add(new Language(languageTag));

			SelectedUiLanguageIndex = GetDefaultLanguageIndex();
		}

		private int GetDefaultLanguageIndex()
		{
			var topUserLanguage = CultureInfo.CurrentUICulture.Name;
			var language = new Language(topUserLanguage);
			var index = AvailableUiLanguages.FindIndex(l => l.NativeName.Equals(language.NativeName));
			return index;
		}
	}
}
