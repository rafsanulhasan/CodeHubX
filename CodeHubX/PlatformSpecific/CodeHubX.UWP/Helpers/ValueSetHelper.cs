using static CodeHubX.UWP.Services.ExecutionService;
using ApplicationTriggerDetails = Windows.ApplicationModel.Background.ApplicationTriggerDetails;
using AppServiceDeferral = Windows.ApplicationModel.AppService.AppServiceDeferral;
using AppServiceRequest = Windows.ApplicationModel.AppService.AppServiceRequest;
using ArgumentException = System.ArgumentException;
using ArgumentNullException = System.ArgumentNullException;
using BackgroundTaskArgument = CodeHubX.Models.BackgroundTaskArgument;
using BackgroundTaskDeferral = Windows.ApplicationModel.Background.BackgroundTaskDeferral;
using ExtendedExecutionSession = Windows.ApplicationModel.ExtendedExecution.ExtendedExecutionSession;
using NotificationsService = CodeHubX.Services.NotificationsService;
using NotNull = JetBrains.Annotations.NotNullAttribute;
using QueryString = Microsoft.QueryStringDotNET.QueryString;
using StringHelper = CodeHubX.Helpers.StringHelper;
using Task = System.Threading.Tasks.Task;
using ToastNotificationActionTriggerDetail = Windows.UI.Notifications.ToastNotificationActionTriggerDetail;
using TypedEventHandler = Windows.Foundation.TypedEventHandler<object, Windows.ApplicationModel.ExtendedExecution.ExtendedExecutionRevokedEventArgs>;
using ValueSet = Windows.Foundation.Collections.ValueSet;

namespace CodeHubX.UWP.Helpers
{
	public static class ValueSetHelper
	{
		private static void ThrowException(this ValueSet valueSet, [NotNull]string key)
		{
			if (!valueSet.TryGetValue(nameof(BackgroundTaskArgument.Action), out var actionObj) || !(actionObj is string action))
			{
				throw new ArgumentException($"App Service requires 'action' parameter");
			}
			else
			{
				throw new ArgumentException($"App Service requires a valid '{key}' parameter for action '{action}'");
			}
		}

		private static TType TryGetValue<TType>(this ValueSet valueSet, string key, bool throwException = true)
		    where TType : class
		{
			TType result = null;
			if (!valueSet.TryGetValue(key, out var obj))
			{
				if (!throwException)
				{
					return null;
				}
				else
				{
					valueSet.ThrowException("key");
				}
			}

			if (!(obj is TType tTypeObj))
			{
				if (!throwException)
				{
					return null;
				}
				else
				{
					throw new ArgumentException($"App Service requires a valid '{key}' parameter");
				}
			}
			result = tTypeObj as TType;

			if (tTypeObj is string strObj)
			{
				if (StringHelper.IsNullOrEmptyOrWhiteSpace(strObj))
				{
					if (!throwException)
					{
						return null;
					}

					valueSet.ThrowException(key);
				}
				else
				{
					if (int.TryParse(strObj, out var intObj))
					{
						result = intObj as TType;
					}
					else if (long.TryParse(strObj, out var longObj))
					{
						result = longObj as TType;
					}
					else if (bool.TryParse(strObj, out var boolObj))
					{
						result = boolObj as TType;
					}
					else
					{
						result = strObj as TType;
					}
				}
			}

			return result;
		}

		internal static bool ValidateValues<TType>(this ValueSet valueSet, string key, out TType value, bool throwException = true, params TType[] expectedValues)
		    where TType : class
		{
			value = valueSet.TryGetValue<TType>(key, throwException);
			var result = true;
			foreach (var v in expectedValues)
			{
				result = value == v;
				if (!result)
				{
					break;
				}
			}
			return result;
		}

		public static async Task Execute<TDeferral>(this ValueSet valueSet, ExtendedExecutionSession session, TypedEventHandler eventHandler, TDeferral deferral)
		{
			var args = valueSet.ToBackgroudTaskArgument();
			if (deferral is BackgroundTaskDeferral bDeferral)
			{
				await session.RunActionAsExtentedAction(async () =>
				{
					await RunActionInUiThread<BackgroundTaskDeferral>(args);
				}, eventHandler, bDeferral);
			}
			else if (deferral is AppServiceDeferral aDeferral)
			{
				await session.RunActionAsExtentedAction(async () =>
				{
					await RunActionInUiThread<BackgroundTaskDeferral>(args);
				}, eventHandler, aDeferral);
			}
		}

		public static ValueSet Parse(object triggerDetailsOrAppServiceRequest)
		{
			if (triggerDetailsOrAppServiceRequest is null)
			{
				throw new ArgumentNullException(nameof(triggerDetailsOrAppServiceRequest));
			}
			var result = new ValueSet();
			if (triggerDetailsOrAppServiceRequest is ToastNotificationActionTriggerDetail toastNotificationActionTrigger)
			{
				result = QueryStringHelper.Parse(toastNotificationActionTrigger).ToValueSet();
			}
			else if (triggerDetailsOrAppServiceRequest is ApplicationTriggerDetails appTrigger)
			{
				result = Validate(appTrigger.Arguments);
			}
			else if (triggerDetailsOrAppServiceRequest is AppServiceRequest appServiceRequest)
			{
				result = Validate(appServiceRequest.Message);
			}

			return result;
		}

		public static BackgroundTaskArgument ToBackgroudTaskArgument(this ValueSet valueSet)
		{
			valueSet = Validate(valueSet);
			var args = new BackgroundTaskArgument();
			foreach (var set in valueSet)
			{
				switch (set.Key)
				{
					case nameof(BackgroundTaskArgument.Action):
						args.Action = set.Value as string;
						break;
					case nameof(BackgroundTaskArgument.What):
						args.What = set.Value as string;
						break;
					case nameof(BackgroundTaskArgument.Where):
						args.Where = set.Value as string;
						break;
					case nameof(BackgroundTaskArgument.Filter):
						args.Filter = set.Value as string;
						break;
					case nameof(BackgroundTaskArgument.WillSendMessage):
						var sendMessage = false;
						if (set.Value is string sendMsgStr && bool.TryParse(sendMsgStr, out var sendMsg))
						{
							sendMessage = sendMsg;
						}
						else if (set.Value is bool sendMsg2)
						{
							sendMessage = sendMsg2;
						}

						args.WillSendMessage = sendMessage;
						break;
					case nameof(BackgroundTaskArgument.State):
						args.State = set.Value as string;
						break;
					case nameof(BackgroundTaskArgument.IsGhost):
						var isGhost = false;
						if (set.Value is string ghostStr && bool.TryParse(ghostStr, out var ghost))
						{
							sendMessage = ghost;
						}
						else if (set.Value is bool ghost2)
						{
							sendMessage = ghost2;
						}

						args.IsGhost = isGhost;
						break;
					case nameof(BackgroundTaskArgument.WillUpdateBadge):
						var willUpdateBadge = false;
						if (set.Value is string badgeStr && bool.TryParse(badgeStr, out var badge))
						{
							sendMessage = badge;
						}
						else if (set.Value is bool badge2)
						{
							sendMessage = badge2;
						}

						args.WillUpdateBadge = willUpdateBadge;
						break;
					case nameof(BackgroundTaskArgument.NotificationId):
						args.NotificationId = set.Value as string;
						break;
					case nameof(BackgroundTaskArgument.MarkNotificationAsRead):
						args.MarkNotificationAsRead = NotificationsService.MarkNotificationAsRead;
						break;
				}
			};

			return args;
		}

		public static QueryString ToQueryString(this ValueSet valueSet)
		{
			if (valueSet is null)
			{
				throw new ArgumentNullException(nameof(valueSet));
			}
			var result = new QueryString();
			if (valueSet.Count > 0)
			{
				foreach (var set in valueSet)
				{
					string value = null;

					if (set.Value is string strValue)
					{
						value = strValue;
					}
					else if (set.Value is bool boolValue)
					{
						value = boolValue ? bool.TrueString : bool.FalseString;
					}
					else
					{
						value = set.Value.ToString();
					}

					result.Add(set.Key, value);
				}
			}

			return result;
		}

		public static ValueSet Validate(ValueSet valueSet)
		{
			if (valueSet.ValidateValues(nameof(BackgroundTaskArgument.Action), out var action, expectedValues: new[] { "sync", "show" }))
			{
				switch (action.ToLower())
				{
					case "sync":
						if (valueSet.ValidateValues(nameof(BackgroundTaskArgument.What), out var syncWhat, expectedValues: new[] { "notifications" }))
						{
							if (valueSet.ValidateValues(nameof(BackgroundTaskArgument.Filter), out var filter, expectedValues: new[] { "all", "participating", "unread" }))
							{
								if (!valueSet.ValidateValues(nameof(BackgroundTaskArgument.WillSendMessage), out var sendMessage, false, new[] { bool.TrueString, bool.FalseString }))
								{
									valueSet.Add(nameof(BackgroundTaskArgument.WillSendMessage), bool.FalseString);
								}
							}
						}
						break;
					case "show":
						if (valueSet.ValidateValues(nameof(BackgroundTaskArgument.What), out var showWhat, expectedValues: new[] { "notifications" }))
						{
							if (valueSet.ValidateValues(nameof(BackgroundTaskArgument.Where), out var where, expectedValues: new[] { "toast", "tiles" }))
							{
								if (where == "toast")
								{
									if (!
	    valueSet.ValidateValues(nameof(BackgroundTaskArgument.IsGhost), out var isGhost, false, new[] { bool.TrueString, bool.FalseString }))
									{
										valueSet.Add(nameof(BackgroundTaskArgument.IsGhost), bool.FalseString);
									}
								}
								else if (where == "tiles")
								{
									if (!
	    valueSet.ValidateValues(nameof(BackgroundTaskArgument.WillUpdateBadge), out var updateBadge, false, new[] { bool.TrueString, bool.FalseString }))
									{
										valueSet.Add(nameof(BackgroundTaskArgument.WillUpdateBadge), bool.TrueString);
									}
								}
							}
						}
						break;
					case "mark":
						if (valueSet.ValidateValues(nameof(BackgroundTaskArgument.Action), out var markWhat, expectedValues: new[] { "notifications", "notification" }))
						{
							if (markWhat == "notification" && valueSet.ValidateValues(nameof(BackgroundTaskArgument.NotificationId), out string notificationId))
							{
								valueSet.Add(nameof(BackgroundTaskArgument.NotificationId), notificationId);
							}
							else
							{
								throw new ArgumentException($"'{nameof(BackgroundTaskArgument.NotificationId)}' must be provided to mark a notification as red");
							}
							if (!valueSet.ValidateValues(nameof(BackgroundTaskArgument.WillSendMessage), out var sendMessageStr, expectedValues: new[] { bool.TrueString, bool.FalseString }))
							{
								valueSet.Add(nameof(BackgroundTaskArgument.WillSendMessage), bool.FalseString);
							}
						}
						break;
				}
			}

			return valueSet;
		}
	}
}
