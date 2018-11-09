using CodeHubX.Helpers;
using Octokit;
using System.Threading.Tasks;

namespace CodeHubX.Services
{
	public class OrganizationsUtility
	{
		public static async Task<Organization> GetOrganizationInfo(string login)
		{
			try
			{
				return await GlobalHelper.GithubClient.Organization.Get(login);
			}
			catch { return null; }
		}
	}
}
