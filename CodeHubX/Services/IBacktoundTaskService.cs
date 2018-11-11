using System.Threading.Tasks;

namespace CodeHubX.Services
{
	public enum NotificationType
	{
		Badge, Toast, Tiles
	}

	public interface IBacktoundTaskService
	{
		Task SyncAllNotifications(bool sendMessage = true);
		Task SyncUnreadNotifications(bool sendMessage = true);
		Task SyncParticipatingNotifications(bool sendMessage = true);

		Task ShowNotifications(NotificationType type);
	}
}