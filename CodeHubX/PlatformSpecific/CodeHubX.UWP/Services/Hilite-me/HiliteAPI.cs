﻿using CodeHubX.Helpers;
using CodeHubX.Services.Hilite_me;
using CodeHubX.UWP.Helpers;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace CodeHubX.UWP.Services.Hilite_me
{
	/// <summary>
	/// A static class that makes API calls to the Hilite.me web service
	/// </summary>
	public static class HiliteAPI
	{
		/// <summary>
		/// Gets the web API URL
		/// </summary>
		public const string APIUrl = "http://hilite.me/api";

		/// <summary>
		/// Gets the fall-back lexer used for unknown file extensions
		/// </summary>
		private const string FallbackLexer = "c";

		/// <summary>
		/// Gets a collection of lexers for less common source code files extensions
		/// </summary>
		public static readonly IReadOnlyDictionary<string, string> UncommonExtensions = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
		{
			  { ".h", "c" },
			  { ".xaml", "xml" },
			  { ".cs", "csharp" },
			  { ".gitignore", "c" },
			  { ".gitattributes", "c" },
			  { ".jshintrc", "c" },
			  { ".yml", "c" },
			  { ".snyk", "c" },
			  { ".lock", "c" },
			  { ".sln", "c" },
			  { ".eslintrc", "json" },
			  { ".babelrc", "json" },
			  { ".bowerrc", "json" },
			  { ".editorconfig", "c" },
			  { ".eslintignore", "c" }
		});

		/// <summary>
		/// Tries to get the highlighted HTML code for a given source code
		/// </summary>
		/// <param name="code">The code to highlight</param>
		/// <param name="path">The path of the file that contains the input code</param>
		/// <param name="style">The requested highlight style</param>
		/// <param name="lineNumbers">Indicates whether or not to show line numbers in the result HTML</param>
		/// <param name="token">The cancellation token for the operation</param>
		[ItemCanBeNull]
		public static async Task<string> TryGetHighlightedCodeAsync([NotNull] string code, [NotNull] string path,
		    SyntaxHighlightStyleEnum style, bool lineNumbers, CancellationToken token)
		{
			// Check if the code is possibly invalid
			const int threshold = 50, length = 1000;
			if (code.Substring(0, length > code.Length ? code.Length : length).Count(
			    c => char.IsControl(c) && c != '\n' && c != '\r' && c != '\t') > threshold)
				return null;

			// Try to extract the code language
			var match = Regex.Match(path, @".*([.]\w+)");
			if (!match.Success || match.Groups.Count != 2)
				return null;

			string
			    extension = match.Groups[1].Value.ToLowerInvariant(),
			    lexer = UncommonExtensions.ContainsKey(extension)
				     ? UncommonExtensions[extension]
				     : extension.Substring(1); // Remove the leading '.'

			// Prepare the POST request content
			var values = new Dictionary<string, string>
			{
				 { "code", code }, // The code to highlight
				 { "lexer", lexer }, // The code language
				 { "style", style.ToString().ToLowerInvariant() }, // The requested syntax highlight style
				 { "divstyles", "border:solid gray;border-width:.0em .0em .0em .0em;padding:.2em .6em;" }, // Default CSS properties
				 { "linenos", lineNumbers ? "pls" : string.Empty } // Includes the line numbers if not empty
			};

			// Make the POST
			var result = await HTTPHelper.POSTWithCacheSupportAsync(APIUrl, values, token);

			// Check if the lexer is unsupported
			if (result.StatusCode ==  System.Net.HttpStatusCode.InternalServerError)
			{
#if DEBUG
				//For debugging, inform if an unsupported extension is found
				System.Diagnostics.Debug.WriteLine($"Possible unsupported extension: {extension} > {lexer}");
#endif
				// Retry with the fall-back lexer
				values["lexer"] = FallbackLexer;
				return (await HTTPHelper.POSTWithCacheSupportAsync(APIUrl, values, token)).Result;
			}

			// Return the result
			return result.Result;
		}
	}
}
