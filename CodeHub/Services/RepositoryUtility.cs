using CodeHub.Helpers;
using CodeHub.Models;
using HtmlAgilityPack;
using JetBrains.Annotations;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using static CodeHub.ViewModels.TrendingViewmodel;

namespace CodeHub.Services
{
	internal class RepositoryUtility
	{
		/// <summary>
		/// Returns trending repositories. First call (second parameter = true) returns first 7 repositories, 
		/// Second call (second parameter = false) returns the rest
		///</summary>
		/// <param name="range">Today, weekly or monthly</param>
		/// <param name="firstCall">Indicates if this is the first call in incremental calls or not</param>
		/// <returns>Trending Repositories in a Time range</returns>
		public static async Task<ObservableCollection<Repository>> GetTrendingRepos(TimeRange range, bool firstCall)
		{
			try
			{
				var repos = new ObservableCollection<Repository>();
				var repoNames = new List<(string, string)>();

				if (firstCall)
					repoNames = await HtmlParseService.ExtractTrendingRepos(range);
				else
				{
					switch (range)
					{
						case TimeRange.TODAY:
							repoNames = GlobalHelper.TrendingTodayRepoNames;
							break;
						case TimeRange.WEEKLY:
							repoNames = GlobalHelper.TrendingWeekRepoNames;
							break;
						case TimeRange.MONTHLY:
							repoNames = GlobalHelper.TrendingMonthRepoNames;
							break;
					}
				}

				if (firstCall)
					for (var i = 0; i < 7; i++)
						repos.Add(await GlobalHelper.GithubClient.Repository.Get(repoNames[i].Item1, repoNames[i].Item2));
				else
					for (var i = 7; i < repoNames.Count; i++)
						repos.Add(await GlobalHelper.GithubClient.Repository.Get(repoNames[i].Item1, repoNames[i].Item2));

				return repos;
			}
			catch { return null; }
		}

		/// <summary>
		/// Gets names of all branches of a given repository 
		/// </summary>
		/// <param name="repo"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<string>> GetAllBranches(Repository repo)
		{
			try
			{
				var branches = await GlobalHelper.GithubClient.Repository.Branch.GetAll(repo.Owner.Login, repo.Name);

				var branchList = new ObservableCollection<string>();
				foreach (var i in branches)
					branchList.Add(i.Name);
				return branchList;
			}
			catch { return null; }
		}

		/// <summary>
		/// Gets a specified Repository
		/// </summary>
		/// <param name="ownerName"></param>
		/// <param name="repoName"></param>
		/// <returns></returns>
		public static async Task<Repository> GetRepository(string ownerName, string repoName)
		{
			try
			{
				return await GlobalHelper.GithubClient.Repository.Get(ownerName, repoName);
			}
			catch { return null; }
		}

		/// <summary>
		/// Gets a specified Repository
		/// </summary>
		/// <param name="repoId"></param>
		/// <returns></returns>
		public static async Task<Repository> GetRepository(long repoId)
		{
			try
			{
				return await GlobalHelper.GithubClient.Repository.Get(repoId);
			}
			catch { return null; }
		}

		#region Files info parsing

		/// <summary>
		/// Returns a wrapped repository content with all the additional info that can be retrieved from the associated HTML page
		/// </summary>
		/// <param name="contentTask">The task with the contents to load</param>
		/// <param name="htmlUrl">The URL to the repository page</param>
		/// <param name="client">The GitHubClient to manually retrieve the commits</param>
		/// <param name="repoId">The id of the current repository</param>
		/// <param name="branch">The name of the current branch to load</param>
		/// <param name="token">The cancellation token for the operation</param>
		[ItemCanBeNull]
		public static async Task<IEnumerable<RepositoryContentWithCommitInfo>> TryLoadLinkedCommitDataAsync(
		    [NotNull] Task<IReadOnlyList<RepositoryContent>> contentTask, [NotNull] string htmlUrl,
		    [NotNull] GitHubClient client, long repoId, [NotNull] string branch, CancellationToken token)
		{
			// Try to download the file info
			IReadOnlyList<RepositoryContent> contents = null;
			try
			{
				// Load the full HTML body
				var view = new WebView();
				var tcs = new TaskCompletionSource<string>();
				view.NavigationCompleted += (s, e) =>
				{
					view.InvokeScriptAsync("eval", new[] { "document.documentElement.outerHTML;" }).AsTask().ContinueWith(t =>
				 {
					 tcs.SetResult(t.Status == TaskStatus.RanToCompletion ? t.Result : null);
				 });
				};

				// Manually set the user agent to get the full desktop site
				var userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; ARM; Trident/7.0; Touch; rv:11.0; WPDesktop) like Gecko";
				var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(htmlUrl));
				httpRequestMessage.Headers.Append("User-Agent", userAgent);
				view.NavigateWithHttpRequestMessage(httpRequestMessage);

				// Run the web calls in parallel
				await Task.WhenAll(contentTask, tcs.Task);
				contents = contentTask.Result;
				var html = tcs.Task.Result;
				if (token.IsCancellationRequested)
					return contents?.OrderByDescending(entry => entry.Type.Value).Select(content => new RepositoryContentWithCommitInfo(content));

				// Load the HTML document
				var document = new HtmlDocument();
				document.LoadHtml(html);

				/* =================
				 * HTML error tags
				 * =================
				 * ...
				 * <include-fragment class="commit-tease commit-loader">
				 * ...
				 *   <div class="loader-error"/>
				 * </include-fragment>
				 * ... */

				// Check if the HTML loading was successful
				if (document.DocumentNode
				    ?.Descendants("include-fragment") // Get the <include-fragment/> nodes
				    ?.FirstOrDefault(node => node.Attributes?.AttributesWithName("class") // Get the nodes with a class attribute
					   ?.FirstOrDefault(att => att.Value?.Equals("commit-tease commit-loader") == true) // That attribute must have this name
					   != null) // There must be a node with these specs if the HTML loading failed
				    ?.Descendants("div") // Get the inner <div/> nodes
				    ?.FirstOrDefault(node => node.Attributes?.AttributesWithName("class")?.FirstOrDefault() // Check the class name
					   ?.Value?.Equals("loader-error") == true) != null || // Make sure there was in fact a loading error
					   html.Contains("class=\"warning include - fragment - error\"") ||
					   html.Contains("Failed to load latest commit information"))
				{
					System.Diagnostics.Debug.WriteLine("[DEBUG] Fallback");
					// Use the Oktokit APIs to get the info
					var tasks = contents.Select(
						r => client.Repository.Commit.GetAll(
							repoId,
							new CommitRequest { Path = r.Path, Sha = branch }, // Only get the commits that edited the current file
							new ApiOptions { PageCount = 1, PageSize = 1 })); // Just get the latest commit for this file
					var commits = await Task.WhenAll(tasks);

					// Query the results
					return contents.AsParallel().OrderByDescending(file => file.Type.Value).Select((file, i) =>
					{
						var commit = commits[i].FirstOrDefault();
						return commit != null
						? new RepositoryContentWithCommitInfo(file, commit, null, commit.Commit.Committer.Date.DateTime)
						: new RepositoryContentWithCommitInfo(file);
					});
				}
				System.Diagnostics.Debug.WriteLine("[DEBUG] HTML parsing");

				/* ================
				 * HTML STRUCTURE
				 * ================ 
				 * ...
				 * <tr class="js-navigation-item">
				 *   ...
				 *   <td class="content">
				 *     <span ...>
				 *       <a href="CONTENT_URL">...</a>
				 *     </span>
				 *   </td>
				 *   <td class="message">
				 *     <span ...>
				 *       [...]?
				 *       <a title="COMMIT_MESSAGE">...</a>
				 *     </span>
				 *   </td>
				 *   <td class="age">
				 *     <span ...>
				 *       <time-ago datetime="EDIT_TIME">...</a>
				 *     </span>
				 *   </td> 
				 * ... */

				// Try to extract the commit info, in parallel
				var cores = Environment.ProcessorCount;
				var partials =
				    (from i in Enumerable.Range(1, cores)
					let list = new List<RepositoryContentWithCommitInfo>()
					select list).ToArray();
				var result = Parallel.For(0, cores, new ParallelOptions { MaxDegreeOfParallelism = cores }, workerId =>
				{
					var max = contents.Count * (workerId + 1) / cores;
					for (var i = contents.Count * workerId / cores; i < max; i++)
					{
						// Find the right node
						var element = contents[i];
						var target = document.DocumentNode?.Descendants("a")?
										.FirstOrDefault(child => child.Attributes?.AttributesWithName("id")?
										.FirstOrDefault()?.Value?.EndsWith(element.Sha) == true);
						// Parse the node contents
						if (target != null)
						{
							// Get the commit and time nodes
							HtmlNode
							    messageRoot = target.Ancestors("td")?.FirstOrDefault()?.Siblings()?.FirstOrDefault(node => node.Name.Equals("td")),
							    timeRoot = messageRoot?.Siblings()?.FirstOrDefault(node => node.Name.Equals("td"));
							HtmlAttribute
								messageTitle = messageRoot?
												.Descendants("a")?
												.Select(node => node.Attributes?.AttributesWithName("title")?.FirstOrDefault())?
												.FirstOrDefault(node => node != null),
								timestamp = timeRoot?
											.Descendants("time-ago")?
											.FirstOrDefault()?
											.Attributes?
											.AttributesWithName("datetime")?
											.FirstOrDefault();

							// Fix the message, if present
							var message = messageTitle?.Value;
							if (message != null)
							{
								message = WebUtility.HtmlDecode(message); // Remove HTML-encoded characters
								message = Regex.Replace(message, @":[^:]+: ?| ?:[^:]+:", string.Empty); // Remove GitHub emojis
							}

							// Add the parsed contents
							if (timestamp?.Value != null)
							{
								if (DateTime.TryParse(timestamp.Value, out var time))
								{
									partials[workerId].Add(new RepositoryContentWithCommitInfo(element, null, message, time));
									continue;
								}
							}
							partials[workerId].Add(new RepositoryContentWithCommitInfo(element, null, message));
							continue;
						}
						partials[workerId].Add(new RepositoryContentWithCommitInfo(element));
					}
				});
				if (!result.IsCompleted)
					throw new InvalidOperationException();

				return partials.SelectMany(list => list).OrderByDescending(entry => entry.Content.Type.Value);
			}
			catch { return contents?.OrderByDescending(entry => entry.Type.Value).Select(content => new RepositoryContentWithCommitInfo(content)); } // Just return the original content without additional info
		}

		#endregion

		/// <summary>
		/// Gets contents of a given repository, branch and path (Text)
		/// </summary>
		/// <param name="repo"></param>
		/// <param name="path"></param>
		/// <param name="branch"></param>
		/// <returns></returns>
		public static async Task<RepositoryContent> GetRepositoryContentTextByPath(Repository repo, string path, string branch)
		{
			try
			{
				return (await GlobalHelper.GithubClient.Repository.Content.GetAllContentsByRef(repo.Id, path, branch)).First();
			}
			catch { return null; }
		}

		/// <summary>
		/// Gets contents of a given repository and branch
		/// </summary>
		/// <param name="repo"></param>
		/// <param name="branch"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<RepositoryContentWithCommitInfo>> GetRepositoryContent(Repository repo, string branch)
		{
			try
			{
				IEnumerable<RepositoryContentWithCommitInfo> results;

				if (SettingsService.Get<bool>(SettingsKeys.LoadCommitsInfo))
				{
					results = await TryLoadLinkedCommitDataAsync(
					    GlobalHelper.GithubClient.Repository.Content.GetAllContentsByRef(repo.Owner.Login, repo.Name, branch), repo.HtmlUrl,
					    GlobalHelper.GithubClient, repo.Id, branch, CancellationToken.None);

					return new ObservableCollection<RepositoryContentWithCommitInfo>(
							results
								.OrderByDescending(entry => entry.Content.Type.Value)
								.ThenBy(entry => entry.Content.Name)
						  );
				}
				else
				{
					results = from item in await GlobalHelper.GithubClient.Repository.Content.GetAllContentsByRef(repo.Owner.Login, repo.Name, branch)
							select new RepositoryContentWithCommitInfo(item);

					return new ObservableCollection<RepositoryContentWithCommitInfo>(results.OrderByDescending(entry => entry.Content.Type.Value));
				}
			}
			catch { return null; }
		}

		/// <summary>
		/// Gets contents of a given repository, branch and path (HTML)
		/// </summary>
		/// <param name="repo"></param>
		/// <param name="path"></param>
		/// <param name="branch"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<RepositoryContentWithCommitInfo>> GetRepositoryContentByPath(Repository repo, string path, string branch)
		{
			try
			{
				var url = $"{repo.HtmlUrl}/tree/{branch}/{path}";
				IEnumerable<RepositoryContentWithCommitInfo> results;

				if (SettingsService.Get<bool>(SettingsKeys.LoadCommitsInfo))
				{
					results = await TryLoadLinkedCommitDataAsync(
					    GlobalHelper.GithubClient.Repository.Content.GetAllContentsByRef(repo.Id, path, branch), url,
					    GlobalHelper.GithubClient, repo.Id, branch, CancellationToken.None);

					return new ObservableCollection<RepositoryContentWithCommitInfo>(results);
				}
				else
				{
					results = from item in await GlobalHelper.GithubClient.Repository.Content.GetAllContentsByRef(repo.Id, path, branch)
							select new RepositoryContentWithCommitInfo(item);

					return new ObservableCollection<RepositoryContentWithCommitInfo>(results.OrderByDescending(entry => entry.Content.Type.Value));
				}

			}
			catch { return null; }
		}

		#region Issue
		/// <summary>
		/// Gets all issues for a given repository
		/// </summary>
		/// <param name="repoId"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<Issue>> GetAllIssuesForRepo(long repoId, RepositoryIssueRequest filter, int pageIndex)
		{
			try
			{
				var options = new ApiOptions
				{
					PageCount = 1,
					PageSize = 10,
					StartPage = pageIndex
				};
				var issues = await GlobalHelper.GithubClient.Issue.GetAllForRepository(repoId, filter, options);
				return new ObservableCollection<Issue>(issues);
			}
			catch { return null; }

		}
		#endregion

		#region PR
		/// <summary>
		/// Gets all PRs for a given repository
		/// </summary>
		/// <param name="repoId"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<PullRequest>> GetAllPullRequestsForRepo(long repoId, PullRequestRequest filter, int pageIndex)
		{
			try
			{
				var options = new ApiOptions
				{
					PageCount = 1,
					PageSize = 10,
					StartPage = pageIndex
				};
				var prList = await GlobalHelper.GithubClient.PullRequest.GetAllForRepository(repoId, filter, options);
				return new ObservableCollection<PullRequest>(prList);
			}
			catch { return null; }
		}
		#endregion

		/// <summary>
		/// Gets all repositories owned by a given user
		/// </summary>
		/// <param name="login"></param>
		/// <param name="pageIndex"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<Repository>> GetRepositoriesForUser(string login, int pageIndex)
		{
			try
			{
				var options = new ApiOptions
				{
					PageSize = 5,
					PageCount = 1,
					StartPage = pageIndex
				};
				var result = await GlobalHelper.GithubClient.Repository.GetAllForUser(login, options);
				return new ObservableCollection<Repository>(result);
			}
			catch { return null; }

		}

		/// <summary>
		/// Gets all repositories starred by a given user
		/// </summary>
		/// <param name="login"></param>
		/// <param name="pageIndex"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<Repository>> GetStarredRepositoriesForUser(string login, int pageIndex)
		{
			try
			{
				var options = new ApiOptions
				{
					PageSize = 5,
					PageCount = 1,
					StartPage = pageIndex
				};
				var result = await GlobalHelper.GithubClient.Activity.Starring.GetAllForUser(login, options);
				return new ObservableCollection<Repository>(result);
			}
			catch { return null; }
		}

		/// <summary>
		/// Gets the preferred README's HTML for a repository
		/// </summary>
		/// <param name="repoId">Repository Id</param>
		/// <returns></returns>
		public static async Task<string> GetReadmeHTMLForRepository(long repoId)
		{
			try
			{
				return await GlobalHelper.GithubClient.Repository.Content.GetReadmeHtml(repoId);
			}
			catch { return null; }
		}

		/// <summary>
		/// Gets the preferred README for a repository
		/// </summary>
		/// <param name="repoId">Repository Id</param>
		/// <returns></returns>
		public static async Task<Readme> GetReadmeForRepository(long repoId)
		{
			try
			{
				return await GlobalHelper.GithubClient.Repository.Content.GetReadme(repoId);
			}
			catch { return null; }
		}

		/// <summary>
		/// Gets default branch for a given repository
		/// </summary>
		/// <param name="repoId"></param>
		/// <returns></returns>
		public static async Task<string> GetDefaultBranch(long repoId)
		{
			try
			{
				var repo = await GlobalHelper.GithubClient.Repository.Get(repoId);
				return repo.DefaultBranch;
			}
			catch { return null; }
		}

		/// <summary>
		/// Stars a given repository
		/// </summary>
		/// <param name="repo"></param>
		/// <returns></returns>
		public static async Task<bool> StarRepository(Repository repo)
		{
			try
			{
				return await GlobalHelper.GithubClient.Activity.Starring.StarRepo(repo.Owner.Login, repo.Name);
			}
			catch { return false; }
		}

		/// <summary>
		/// Un-stars a given repository
		/// </summary>
		/// <param name="repo"></param>
		/// <returns></returns>
		public static async Task<bool> UnstarRepository(Repository repo)
		{
			try
			{
				return await GlobalHelper.GithubClient.Activity.Starring.RemoveStarFromRepo(repo.Owner.Login, repo.Name);
			}
			catch { return false; }
		}

		/// <summary>
		/// Watches a repository
		/// </summary>
		/// <param name="repo"></param>
		/// <returns></returns>
		public static async Task<bool> WatchRepository(Repository repo)
		{
			try
			{
				return (await GlobalHelper.GithubClient.Activity.Watching.WatchRepo(repo.Id, new NewSubscription { Subscribed = true })).Subscribed;
			}
			catch { return false; }
		}

		/// <summary>
		/// Un-watches a repository
		/// </summary>
		/// <param name="repo"></param>
		/// <returns></returns>
		public static async Task<bool> UnwatchRepository(Repository repo)
		{
			try
			{
				return await GlobalHelper.GithubClient.Activity.Watching.UnwatchRepo(repo.Id);
			}
			catch { return false; }
		}

		/// <summary>
		/// Forks a repository
		/// </summary>
		/// <param name="repo"></param>
		/// <returns>Forked repository</returns>
		public static async Task<Repository> ForkRepository(Repository repo)
		{
			try
			{
				return await GlobalHelper.GithubClient.Repository.Forks.Create(repo.Id, new NewRepositoryFork());
			}
			catch { return null; }
		}

		/// <summary>
		/// Checks if a repository is watched by the authorized user
		/// </summary>
		/// <param name="repo"></param>
		/// <returns></returns>
		public static async Task<bool> CheckWatched(Repository repo)
		{
			try
			{
				return await GlobalHelper.GithubClient.Activity.Watching.CheckWatched(repo.Id);
			}
			catch { return false; }
		}

		/// <summary>
		/// Checks if a repository is starred by the authorized user
		/// </summary>
		/// <param name="repo"></param>
		/// <returns></returns>
		public static async Task<bool> CheckStarred(Repository repo)
		{
			try
			{
				return await GlobalHelper.GithubClient.Activity.Starring.CheckStarred(repo.Owner.Login, repo.Name);
			}
			catch { return false; }
		}

		/// <summary>
		/// Gets all commits for a given file path
		/// </summary>
		/// <param name="repoId">Repository Id</param>
		/// <param name="path">file path</param>
		/// <returns></returns>
		public static async Task<ObservableCollection<GitHubCommit>> GetAllCommitsForFile(long repoId, string path)
		{
			try
			{
				var request = new CommitRequest { Path = path };

				var list = await GlobalHelper.GithubClient.Repository.Commit.GetAll(repoId, request);
				return new ObservableCollection<GitHubCommit>(list);
			}
			catch { return null; }
		}

		/// <summary>
		/// Gets all contributors for a repository
		/// </summary>
		/// <param name="repoId"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<RepositoryContributor>> GetContributorsForRepository(long repoId)
		{
			try
			{
				var options = new ApiOptions
				{
					PageCount = 1,
					PageSize = 100
				};
				var users = await GlobalHelper.GithubClient.Repository.GetAllContributors(repoId, options);
				return new ObservableCollection<RepositoryContributor>(users);
			}
			catch { return null; }
		}

		/// <summary>
		/// Gets all releases for a repository
		/// </summary>
		/// <param name="repoId"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<Release>> GetReleasesForRepository(long repoId)
		{
			try
			{
				var releases = await GlobalHelper.GithubClient.Repository.Release.GetAll(repoId);
				return new ObservableCollection<Release>(releases);
			}
			catch { return null; }
		}
	}
}

