using GalaSoft.MvvmLight;
using Xamarin.Forms;

namespace CodeHubX.Views
{
	public class CustomPage
		: Page
	{
		public static BindableProperty ViewModelProperty
			= BindableProperty.Create(nameof(ViewModel), typeof(ObservableObject), typeof(CustomPage), new ObservableObject());

		public ObservableObject ViewModel
		{
			get => GetValue(ViewModelProperty) as ObservableObject;
			set => SetValue(ViewModelProperty, value);
		}

		public CustomPage()
			: base()
		{
			ViewModel = null;
				
		}

		public CustomPage(ObservableObject viewModel)
			: base()
			=> ViewModel = viewModel;

	}

	public class CustomPage<TViewModel>
		: CustomPage
		where TViewModel : ObservableObject, new()
	{
		public new static BindableProperty ViewModelProperty
			= BindableProperty.Create(nameof(ViewModel), typeof(TViewModel), typeof(CustomPage<TViewModel>), new TViewModel());

		public new TViewModel ViewModel
		{
			get => GetValue(ViewModelProperty) as TViewModel;
			set => SetValue(ViewModelProperty, value);
		}

		public CustomPage()
			: base()
		=> ViewModel = null;

		public CustomPage(TViewModel viewModel)
			: base()
			=> ViewModel = viewModel;
	}

	public class CustomContentPage
		: ContentPage
	{
		public static BindableProperty ViewModelProperty
			= BindableProperty.Create(nameof(ViewModel), typeof(ObservableObject), typeof(CustomContentPage), new ObservableObject());

		public ObservableObject ViewModel
		{
			get => GetValue(ViewModelProperty) as ObservableObject;
			set => SetValue(ViewModelProperty, value);
		}

		public CustomContentPage()
			: base() 
			=> ViewModel = null;

		public CustomContentPage(ObservableObject viewModel)
			: this()
			=> ViewModel = viewModel;
	}

	public class CustomContentPage<TViewModel>
		: CustomContentPage
		where TViewModel : ObservableObject, new()
	{
		public new static BindableProperty ViewModelProperty
			= BindableProperty.Create(nameof(ViewModel), typeof(TViewModel), typeof(CustomPage<TViewModel>), new TViewModel());

		public new TViewModel ViewModel
		{
			get => GetValue(ViewModelProperty) as TViewModel;
			set => SetValue(ViewModelProperty, value);
		}

		public CustomContentPage()
			: base() 
			=> ViewModel = null;

		public CustomContentPage(TViewModel viewModel)
			: this()
			=> ViewModel = viewModel;
	}
}
