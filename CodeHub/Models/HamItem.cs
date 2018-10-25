using GalaSoft.MvvmLight;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CodeHub.Models
{
	public class HamItem : ObservableObject
	{
		private bool _isSelected;

		private Visibility _selectedVisual = Visibility.Collapsed;

		public Type DestPage { get; set; }

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				SelectedVisual = value ? Visibility.Visible : Visibility.Collapsed;
				RaisePropertyChanged("IsSelected");
			}
		}

		public string Label { get; set; }

		public Visibility SelectedVisual
		{
			get => _selectedVisual;
			set
			{
				_selectedVisual = value;
				RaisePropertyChanged("SelectedVisual");
			}
		}

		public Geometry Symbol { get; set; }
	}
}
