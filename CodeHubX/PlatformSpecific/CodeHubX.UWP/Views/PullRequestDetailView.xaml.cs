﻿using CodeHubX.Helpers;
using CodeHubX.UWP.ViewModels;
using Octokit;
using System;
using System.Threading.Tasks;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace CodeHubX.UWP.Views
{

	public sealed partial class PullRequestDetailView : Windows.UI.Xaml.Controls.Page
	{
		public PullRequestDetailViewmodel ViewModel;

		private async void CommentDialogOpen_Tapped(object sender, TappedRoutedEventArgs e) 
			=> await ToggleCommentDialogVisibility(true);

		private async void CancelComment_Tapped(object sender, TappedRoutedEventArgs e) 
			=> await ToggleCommentDialogVisibility(false);

		private async void Comment_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if (!StringHelper.IsNullOrEmptyOrWhiteSpace(ViewModel.CommentText))
			{
				await ToggleCommentDialogVisibility(false);
				ViewModel.CommentCommand.Execute(null);
			}
		}

		private async void Expander_Click(object sender, RoutedEventArgs e)
		{
			if (DetailPanel.Visibility == Visibility.Visible)
			{
				ExpanderIcon.Glyph = "\uE0E5";
				await DetailPanel.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 0.98f, 100, null, 0, EasingFunctionNames.SineEaseInOut);
				DetailPanel.Visibility = Visibility.Collapsed;
			}
			else
			{
				ExpanderIcon.Glyph = "\uE0E4";
				DetailPanel.SetVisualOpacity(0);
				DetailPanel.Visibility = Visibility.Visible;
				await DetailPanel.StartCompositionFadeScaleAnimationAsync(0, 1, 0.98f, 1, 100, null, 0, EasingFunctionNames.SineEaseInOut);
			}
		}

		private async Task ToggleCommentDialogVisibility(bool visible)
		{
			if (visible)
			{
				CommentDialog.SetVisualOpacity(0);
				CommentDialog.Visibility = Visibility.Visible;
				await CommentDialog.StartCompositionFadeScaleAnimationAsync(0, 1, 1.1f, 1, 150, null, 0, EasingFunctionNames.SineEaseInOut);
			}
			else
			{
				await CommentDialog.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
				CommentDialog.Visibility = Visibility.Collapsed;
			}
		}

		private async void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
		{
			if (e.NewState.Name.Equals("Wide") || e.NewState.Name.Equals("Normal"))
			{
				ExpanderIcon.Glyph = "\uE0E4";
				DetailPanel.SetVisualOpacity(0);
				DetailPanel.Visibility = Visibility.Visible;
				await DetailPanel.StartCompositionFadeScaleAnimationAsync(0, 1, 0.98f, 1, 100, null, 0, EasingFunctionNames.SineEaseInOut);
			}
		}


		public PullRequestDetailView()
		{
			InitializeComponent();
			ViewModel = new PullRequestDetailViewmodel();

			DataContext = ViewModel;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			ViewModel.CommentText = string.Empty;
			await ViewModel.Load(e.Parameter as Tuple<Repository, PullRequest>);
		}

		protected override async void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);
			await ToggleCommentDialogVisibility(false);
		}
	}
}
