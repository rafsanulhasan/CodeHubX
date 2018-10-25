using CodeHub.Helpers;
using Octokit;
using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Credentials;


namespace CodeHub.Services
{
	internal class AuthService
	{
		#region App Credentials
		private GitHubClient client = new GitHubClient(new ProductHeaderValue("CodeHub"));
		private readonly Uri endUri = new Uri("http://example.com/path");
		#endregion

		/// <summary>
		/// Opens OAuth window using WebAuthenticationBroker class and returns true is authentication is successful
		/// </summary>
		/// <returns></returns>
		public async Task<bool> Authenticate()
		{
			try
			{
				var clientId = await AppCredentials.GetAppKey();
				var request = new OauthLoginRequest(clientId)
				{
					Scopes = { "user", "repo" },
				};

				var oauthLoginUrl = client.Oauth.GetGitHubLoginUrl(request);

				var WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
												   WebAuthenticationOptions.None,
												   oauthLoginUrl,
												   endUri
												   );
				if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
				{
					var response = WebAuthenticationResult.ResponseData;

					return await Authorize(response);

				}
				else
					return false;
			}
			catch { return false; }

		}

		/// <summary>
		/// Makes call for Access Token
		/// </summary>
		/// <param name="response">Response string containing 'code' token, used for getting access token</param>
		/// <returns></returns>
		private async Task<bool> Authorize(string response)
		{
			try
			{
				var responseData = response.Substring(response.IndexOf("code"));
				var keyValPairs = responseData.Split('=');
				var code = keyValPairs[1].Split('&')[0];


				var clientId = await AppCredentials.GetAppKey();
				var appSecret = await AppCredentials.GetAppSecret();

				var request = new OauthTokenRequest(clientId, appSecret, code);
				var token = await client.Oauth.CreateAccessToken(request);
				if (token != null)
				{
					client.Credentials = new Credentials(token.AccessToken);
					await SaveToken(token.AccessToken, clientId);
				}
				return true;
			}
			catch { return false; }
		}

		/// <summary>
		/// Saves access token in device
		/// </summary>
		/// <param name="token"></param>
		/// <param name="clientId">Client Id is used as resource string for PasswordCredential</param>
		/// <returns></returns>
		private async Task<bool> SaveToken(string token, string clientId)
		{
			try
			{
				GlobalHelper.GithubClient = UserService.GetAuthenticatedClient(token);
				var user = await UserService.GetCurrentUserInfo();

				var vault = new PasswordVault();
				vault.Add(new PasswordCredential(clientId, user.Id.ToString(), token));

				await AccountsService.AddUser(new Models.Account { Id = user.Id, AvatarUrl = user.AvatarUrl, IsLoggedIn = true, Login = user.Login, IsActive = true });

				return true;
			}
			catch { return false; }
		}

		/// <summary>
		/// Gets Access token if stored in device
		/// </summary>
		/// <returns></returns>
		public static string GetToken(string userId)
		{
			try
			{
				var vault = new PasswordVault();
				var credentialList = vault.FindAllByUserName(userId);
				if (credentialList.Count > 0)
				{
					credentialList[0].RetrievePassword();
					return credentialList[0].Password;
				}
				else
				{
					return null;
				}
			}
			catch { return null; }
		}

		/// <summary>
		/// Checks if user's device has an access token
		/// </summary>
		/// <returns></returns>
		public static bool CheckAuth(string userId)
		{
			try
			{
				var token = GetToken(userId);
				if (token != null)
					return true;
				else
					return false;
			}
			catch { return false; }
		}

		/// <summary>
		/// Deletes the access token from user's device
		/// </summary>
		/// <returns></returns>
		public static async Task<bool> SignOut(string userId)
		{
			try
			{
				//var vault = new PasswordVault();
				//var credentialList = vault.FindAllByUserName(userId);

				//if (credentialList.Count > 0)
				//{
				//    vault.Remove(credentialList[0]);
				//}
				await AccountsService.SignOutOfAccount(userId);
				return true;
			}
			catch { return false; }
		}
	}
}
