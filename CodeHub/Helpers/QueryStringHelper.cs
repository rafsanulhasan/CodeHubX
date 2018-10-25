using CodeHub.Services;
using static CodeHub.Services.ExecutionService;
using ApplicationTriggerDetails = Windows.ApplicationModel.Background.ApplicationTriggerDetails;
using AppServiceDeferral = Windows.ApplicationModel.AppService.AppServiceDeferral;
using AppServiceRequest = Windows.ApplicationModel.AppService.AppServiceRequest;
using ArgumentException = System.ArgumentException;
using ArgumentNullException = System.ArgumentNullException;
using BackgroundTaskArgument = CodeHub.Models.BackgroundTaskArgument;
using BackgroundTaskDeferral = Windows.ApplicationModel.Background.BackgroundTaskDeferral;
using ExtendedExecutionSession = Windows.ApplicationModel.ExtendedExecution.ExtendedExecutionSession;
using QueryString = Microsoft.QueryStringDotNET.QueryString;
using Task = System.Threading.Tasks.Task;
using ToastNotificationActionTriggerDetail = Windows.UI.Notifications.ToastNotificationActionTriggerDetail;
using TypedEventHandler = Windows.Foundation.TypedEventHandler<object, Windows.ApplicationModel.ExtendedExecution.ExtendedExecutionRevokedEventArgs>;
using ValueSet = Windows.Foundation.Collections.ValueSet;

namespace CodeHub.Helpers
{
    public static class QueryStringHelper
    {
        private static string TryGetValue(this QueryString query, string key, bool throwException = true)
        {
            if (query.TryGetValue(key, out string result))
            {
                if (StringHelper.IsNullOrEmptyOrWhiteSpace(result))
                {
                    if (!throwException)
                    {
                        return null;
                    }
                    else
                    {
                        if (!query.TryGetValue(nameof(BackgroundTaskArgument.Action), out string action))
                        {
                            throw new ArgumentException($"App Service requires 'action' parameter");
                        }
                        else
                        {
                            throw new ArgumentException($"App Service requires a valid '{key}' parameter for action '{action}'");
                        }
                    }
                }
                else
                {
                    return result;
                }
            }
            else
            {
                if (throwException)
                {
                    if (!query.TryGetValue(nameof(BackgroundTaskArgument.Action), out string action))
                    {
                        throw new ArgumentException($"App Service requires 'action' parameter");
                    }
                    else
                    {
                        throw new ArgumentException($"App Service requires a valid '{key}' parameter for action '{action}'");
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        internal static bool ValidateValues(this QueryString query, string key, out string value, bool throwException = true, params string[] expectedValues)
        {
            value = query.TryGetValue(key, throwException);
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

        public static async Task Execute<TDeferral>(this QueryString query, ExtendedExecutionSession session, TypedEventHandler eventHandler, TDeferral deferral)
        {
            var args = query.ToBackgroudTaskArgument();
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

        public static QueryString Parse(object triggerDetailsOrAppServiceRequest)
        {
            if (triggerDetailsOrAppServiceRequest is null)
            {
                throw new ArgumentNullException(nameof(triggerDetailsOrAppServiceRequest));
            }
            QueryString result = null;
            if (triggerDetailsOrAppServiceRequest is string strObject)
            {
                result = QueryString.Parse(strObject);
            }
            else if (triggerDetailsOrAppServiceRequest is ToastNotificationActionTriggerDetail toastNotificationActionTrigger)
            {
                result = Parse(QueryString.Parse(toastNotificationActionTrigger.Argument));
            }
            else if (triggerDetailsOrAppServiceRequest is ApplicationTriggerDetails appTrigger)
            {
                result = ValueSetHelper.Parse(appTrigger).ToQueryString();
            }
            else if (triggerDetailsOrAppServiceRequest is AppServiceRequest appSvcRequest)
            {
                result = ValueSetHelper.Parse(appSvcRequest).ToQueryString();
            }
            else
            {
                throw new ArgumentException($"Invalid object");
            }
            return result;
        }

        public static BackgroundTaskArgument ToBackgroudTaskArgument(this QueryString query)
        {
            query = Validate(query);

            var args = new BackgroundTaskArgument();
            foreach (var q in query)
            {
                switch (q.Name)
                {
                    case nameof(BackgroundTaskArgument.Action):
                        args.Action = q.Value as string;
                        break;
                    case nameof(BackgroundTaskArgument.What):
                        args.What = q.Value as string;
                        break;
                    case nameof(BackgroundTaskArgument.Where):
                        args.Where = q.Value as string;
                        break;
                    case nameof(BackgroundTaskArgument.Filter):
                        args.Filter = q.Value as string;
                        break;
                    case nameof(BackgroundTaskArgument.WillSendMessage):
                        args.WillSendMessage = q.Value == bool.TrueString;
                        break;
                    case nameof(BackgroundTaskArgument.State):
                        args.State = q.Value;
                        break;
                    case nameof(BackgroundTaskArgument.IsGhost):
                        args.IsGhost = q.Value == bool.TrueString;
                        break;
                    case nameof(BackgroundTaskArgument.WillUpdateBadge):
                        args.WillUpdateBadge = q.Value == bool.TrueString;
                        break;
                    case nameof(BackgroundTaskArgument.NotificationId):
                        args.NotificationId = q.Value as string;
                        break;
                }
            }

            return args;
        }

        public static ValueSet ToValueSet(this QueryString query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var valueSet = new ValueSet();
            if (query.Count() > 0)
            {
                foreach (var q in query)
                {
                    valueSet.Add(q.Name, q.Value);
                };
            }

            return valueSet;
        }

        public static QueryString Validate(QueryString query)
        {
            if (query.ValidateValues(nameof(BackgroundTaskArgument.Action), out string action, expectedValues: new[] { "sync", "show", "mark" }))
            {
                switch (action.ToLower())
                {
                    case "sync":
                        if (query.ValidateValues(nameof(BackgroundTaskArgument.What), out string syncWhat, expectedValues: new[] { "notifications" }))
                        {

                            if (query.ValidateValues(nameof(BackgroundTaskArgument.Filter), out string filter, expectedValues: new[] { "all", "participating", "unread" }))
                            {
                                if (!query.ValidateValues(nameof(BackgroundTaskArgument.WillSendMessage), out string sendMessageStr, expectedValues: new[] { bool.TrueString, bool.FalseString }))
                                {
                                    query.Add(nameof(BackgroundTaskArgument.WillSendMessage), bool.FalseString);
                                }
                            }
                        }
                        break;
                    case "show":
                        if (query.ValidateValues(nameof(BackgroundTaskArgument.What), out string showWhat, expectedValues: new[] { "notifications" }))
                        {
                            if (query.ValidateValues(nameof(BackgroundTaskArgument.Where), out string where, expectedValues: new[] { "toast", "tiles" }))
                            {
                                if (where == "toast")
                                {
                                    if (!
query.ValidateValues(nameof(BackgroundTaskArgument.IsGhost), out string isGhost, false, new[] { bool.TrueString, bool.FalseString }))
                                    {
                                        query.Add(nameof(BackgroundTaskArgument.IsGhost), bool.FalseString);
                                    }
                                }
                                else if (where == "tiles")
                                {
                                    if (!
query.ValidateValues(nameof(BackgroundTaskArgument.WillUpdateBadge), out string updateBadge, false, new[] { bool.TrueString, bool.FalseString }))
                                    {
                                        query.Add(nameof(BackgroundTaskArgument.WillUpdateBadge), bool.TrueString);
                                    }
                                }
                            }
                        }
                        break;
                    case "mark":
                        if (query.ValidateValues(nameof(BackgroundTaskArgument.What), out string markWhat, expectedValues: new[] { "notifications", "notification" }))
                        {
                            if (markWhat == "notification" && query.TryGetValue(nameof(BackgroundTaskArgument.NotificationId), out string notificationId))
                            {
                                query.Add(nameof(BackgroundTaskArgument.NotificationId), notificationId);
                            }
                            else
                            {
                                throw new ArgumentException($"'{nameof(BackgroundTaskArgument.NotificationId)}' must be provided to mark a notification as red");
                            }
                            if (!query.ValidateValues(nameof(BackgroundTaskArgument.WillSendMessage), out string sendMessageStr, expectedValues: new[] { bool.TrueString, bool.FalseString }))
                            {
                                query.Add(nameof(BackgroundTaskArgument.WillSendMessage), bool.FalseString);
                            }
                        }
                        break;
                }
            }
            return query;
        }
    }
}
