using CodeHubX.Helpers;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;

namespace CodeHubX.ViewModels.Settings
{
	public abstract class AboutSettingsViewModelBase
	{
		public string Logo { get; protected set; }

		public string DisplayName { get; protected set; }

		public string Publisher { get; protected set; }

		public string Version { get; protected set; }

		private ICommand _shoWWhatsNewCommand;
		public ICommand ShoWWhatsNewCommand
		{
			get
			{
				if (_shoWWhatsNewCommand == null)
					_shoWWhatsNewCommand = new RelayCommand(() => Messenger.Default.Send(new GlobalHelper.ShowWhatsNewPopupMessageType()));

				return _shoWWhatsNewCommand;
			}
		}


		public AboutSettingsViewModelBase() 
			=> Logo = "/Assets/Images/appLogoPurple.png";
	}
}
