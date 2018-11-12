﻿using System;
using System.Threading.Tasks;

namespace CodeHubX.Services
{
	/// <summary>
	/// An interface for an asynchronous navigations system (due to the navigation animations)
	/// </summary>
	public interface IAsyncNavigationService
	{
		/// <summary>
		/// Navigates to the target page
		/// </summary>
		/// <param name="pageType">The type of the target page</param>
		/// <param name="parameter">The optional parameter</param>
		/// <param name="title">The desired but optional title of the target page.</param>
		/// <param name="backPageType">The optional type of the back page</param>
		/// <param name="backPageParameter">The optional parameter for the back page</param>
		/// <param name="backPageTitle">The desired but optional title of the back page.</param>
		/// <param name="shouldClearBackStack">Set it to 'False' if you don't want to clear the back stack.</param>
		Task<bool> NavigateAsync(Type pageType, object parameter = null, string pageTitle = null, Type backPageType = null, object backPageParameter = null, string backPageTitle = null, bool shouldClearBackStack = true);

		/// <summary>
		/// Navigates to the target page without displaying any animations
		/// </summary>
		/// <param name="pageType">The type of the target page</param>
		/// <param name="pageTitle">The page title</param>
		void NavigateWithoutAnimations(Type pageType, string pageTitle);

		/// <summary>
		/// Navigates to the target page with a given parameter without displaying any animations
		/// </summary>
		/// <param name="pageType">The type of the target page</param>
		/// <param name="pageTitle">The page title</param>
		void NavigateWithoutAnimations(Type pageType, string pageTitle, object parameter);

		/// <summary>
		/// Gets the current page type
		/// </summary>
		Type CurrentSourcePageType { get; }

		/// <summary>
		/// Tries to navigate back
		/// </summary>
		Task<bool> GoBackAsync();

		/// <summary>
		/// Checks if it is possible to perform a back navigation
		/// </summary>
		Task<bool> CanGoBackAsync();

		/// <summary>
		/// Adds page to the navigation history of the frame
		/// </summary>
		/// <param name="pageType">The type of the back page</param>
		/// <param name="parameter">The optional parameter</param>
		/// <param name="title">The desired but optional title of the back page.</param>
		void AddToBackStack(Type sourcePageType, object parameter = null, string title = null);

		/// <summary>
		/// Clears the navigation history of the frame
		/// </summary>
		void ClearBackStack();

		/// <summary>
		/// Search for the Page Title with the given Menu type
		/// </summary>
		/// <param name="type">type of the Menu</param>
		/// <returns>string</returns>
		/// <exception cref="Exception">When the given type don't have a Page Title pair</exception> 
		string ChoosePageTitleByPageType(Type type);
	}
}
