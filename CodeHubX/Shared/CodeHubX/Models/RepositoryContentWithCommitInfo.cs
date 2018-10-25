﻿using JetBrains.Annotations;
using Octokit;
using System;
using System.Text.RegularExpressions;

namespace CodeHubX.Models
{
	/// <summary>
	/// A class that wraps a repository content and its linked commit
	/// </summary>
	public sealed class RepositoryContentWithCommitInfo
	{
		/// <summary>
		/// Gets the repository content for this instance
		/// </summary>
		[NotNull]
		public RepositoryContent Content { get; }

		/// <summary>
		/// Gets the last edit time for this instance, if available
		/// </summary>
		[CanBeNull]
		public DateTime? LastEditTime { get; }

		/// <summary>
		/// Gets the linked commit, if available
		/// </summary>
		[CanBeNull]
		public GitHubCommit Commit { get; }
		
		// Private message field (shortcut to avoid repeated API calls)
		private readonly string _CommitMessage;

		/// <summary>
		/// Gets the available commit message, if present
		/// </summary>
		[CanBeNull]
		public string CommitMessage
		{
			get
			{
				if (Commit?.Commit.Message != null)
				{
					return Regex.Replace(Commit.Commit.Message, @":[^:]+: ?| ?:[^:]+:", string.Empty);
				}
				return _CommitMessage;
			}
		}

		public RepositoryContentWithCommitInfo([NotNull] RepositoryContent content, [CanBeNull] GitHubCommit commit = null, [CanBeNull] string message = null, [CanBeNull] DateTime? editTime = null)
		{
			Content = content;
			Commit = commit;
			_CommitMessage = message;
			LastEditTime = editTime;
		}

		// Implicit converter for the content
		public static implicit operator RepositoryContent([NotNull] RepositoryContentWithCommitInfo instance) 
			=> instance.Content;
	}
}
