using CodeHubX.UWP.Services;
using GalaSoft.MvvmLight;

namespace CodeHubX.UWP.ViewModels.Settings
{
	public class NofiticationSettingsViewModel : ObservableObject
	{
		private bool _isBadgeEnabled = SettingsService.Get<bool>(SettingsKeys.IsLiveTilesBadgeEnabled);
		private bool _isLiveTilesEnabled = SettingsService.Get<bool>(SettingsKeys.IsLiveTilesEnabled);
		private bool _isAllBadgesUpdateEnabled = SettingsService.Get<bool>(SettingsKeys.IsLiveTileUpdateAllBadgesEnabled);
		private bool _isToastEnabled = SettingsService.Get<bool>(SettingsKeys.IsToastEnabled);

		public bool IsBadgeEnabled
		{
			get => _isBadgeEnabled;
			set
			{
				if (_isBadgeEnabled != value)
				{
					_isBadgeEnabled = value;
					SettingsService.Save(SettingsKeys.IsLiveTilesBadgeEnabled, value);
					RaisePropertyChanged(() => IsBadgeEnabled);
				}
			}
		}

		public bool IsLiveTilesEnabled
		{
			get => _isLiveTilesEnabled;
			set
			{
				if (_isLiveTilesEnabled != value)
				{
					_isLiveTilesEnabled = value;
					IsBadgeEnabled = !value ? false : IsBadgeEnabled;
					SettingsService.Save(SettingsKeys.IsLiveTilesEnabled, value);
					RaisePropertyChanged(() => IsLiveTilesEnabled);
				}
			}
		}

		public bool IsAllBadgesUpdateEnabled
		{
			get => _isAllBadgesUpdateEnabled;
			set
			{
				if (_isAllBadgesUpdateEnabled != value)
				{
					_isAllBadgesUpdateEnabled = value;
					SettingsService.Save(SettingsKeys.IsLiveTileUpdateAllBadgesEnabled, value);
					RaisePropertyChanged(() => IsAllBadgesUpdateEnabled);
				}
			}
		}
		public bool IsToastEnabled
		{
			get => _isToastEnabled;
			set
			{
				if (_isToastEnabled != value)
				{
					_isToastEnabled = value;
					SettingsService.Save(SettingsKeys.IsToastEnabled, value);
					RaisePropertyChanged(() => IsToastEnabled);
				}
			}
		}

		public NofiticationSettingsViewModel()
		{
		}
	}
}
