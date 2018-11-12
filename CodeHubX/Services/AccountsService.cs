using CodeHubX.Helpers;
using CodeHubX.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeHubX.Services
{
	public class AccountsService
	{
		private const string SETTINGS_FILENAME = "Settings.json";
		private static readonly string SETTINGS_FILEPATH = $"{Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), SETTINGS_FILENAME)}";

		/// <summary>
		/// Get all available accounts
		/// </summary>
		/// <returns></returns>
		public static async Task<ObservableCollection<Account>> GetAllUsers()
			=> await Task.Run(() =>
			{
				try
				{
					var content = File.ReadAllText(SETTINGS_FILEPATH);
					return StringHelper.IsNullOrEmptyOrWhiteSpace(content)
						? null
						: JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
				}
				catch { return null; }
			});

		/// <summary>
		/// Adds an account to list of available accounts
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static async Task<bool> AddUser(Account user)
			=> await Task.Run(() =>
			{
				try
				{
					var content = File.ReadAllText(SETTINGS_FILEPATH);
					var allUsers = JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
					if (allUsers != null)
					{
						var sameUser = allUsers.Where(x => x.Id == user.Id);
						if (sameUser.Count() == 0)
						{
							//mark all existing users as inactive and add new active user
							allUsers.ForEach(u => u.IsActive = false);
							allUsers.Add(user);
							File.WriteAllText(SETTINGS_FILEPATH, JsonConvert.SerializeObject(allUsers));
						}
						else
						{
							//The user already exists
							allUsers.Where(x => x.Id == user.Id).First().IsActive = true;
							File.WriteAllText(SETTINGS_FILEPATH, JsonConvert.SerializeObject(allUsers));
						}
					}
					else
						File.WriteAllText(SETTINGS_FILEPATH, JsonConvert.SerializeObject(new ObservableCollection<Account> { user }));
					return true;
				}
				catch { return false; }
			});

		/// <summary>
		/// Deletes an account from list of available accounts
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static async Task<bool> RemoveUser(string userId)
			=> await Task.Run(() =>
			{
				try
				{
					var content = File.ReadAllText(SETTINGS_FILEPATH);
					var users = JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
					users.Remove(users.Where(x => x.Id.ToString() == userId).First());
					File.WriteAllText(SETTINGS_FILEPATH, JsonConvert.SerializeObject(users));
					return true;
				}
				catch { return false; }
			});

		/// <summary>
		/// Marks an account as Inactive
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static async Task<bool> SignOutOfAccount(string userId)
			=> await Task.Run(() =>
			{
				try
				{
					var content = File.ReadAllText(SETTINGS_FILEPATH);

					var users = JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
					users.Where(x => x.Id.ToString() == userId).First().IsActive = false;
					File.WriteAllText(SETTINGS_FILEPATH, JsonConvert.SerializeObject(users));
					return true;
				}
				catch { return false; }
			});

		/// <summary>
		/// Marks an account as active
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static async Task<bool> MakeAccountActive(string userId)
			=> await Task.Run(() =>
			{
				try
				{
					var content = File.ReadAllText(SETTINGS_FILEPATH);
					var users = JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
					users.ForEach(u => u.IsActive = u.Id.ToString() == userId);
					File.WriteAllText(SETTINGS_FILEPATH, JsonConvert.SerializeObject(users));
					return true;
				}
				catch { return false; }
			});
	}
}
