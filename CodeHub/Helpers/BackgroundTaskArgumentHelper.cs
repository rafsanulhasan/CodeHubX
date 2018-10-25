using CodeHub.Models;
using Microsoft.QueryStringDotNET;
using Windows.Foundation.Collections;

namespace CodeHub.Helpers
{
	public static class BackgroundTaskArgumentHelper
	{
		public static T To<T>(this BackgroundTaskArgument args)
		    where T : class
		{
			if (typeof(T) == typeof(ValueSet))
			{

				var valueSet = new ValueSet
				{
					{ nameof(args.Action), args.Action },
					{ nameof(args.Filter), args.Filter },
					{ nameof(args.WillSendMessage), args.WillSendMessage },
					{ nameof(args.Where), args.Where },
					{ nameof(args.WillUpdateBadge), args.WillUpdateBadge },
					{ nameof(args.IsGhost), args.IsGhost },
					{ nameof(args.State), args.State }
				};

				return valueSet as T;
			}

			else if (typeof(T) == typeof(QueryString))
			{
				var query = new QueryString
				{
					{ nameof(args.Action), args.Action },
					{ nameof(args.Filter), args.Filter },
					{ nameof(args.WillSendMessage), args.WillSendMessage ? bool.TrueString : bool.FalseString },
					{ nameof(args.Where), args.Where },
					{ nameof(args.WillUpdateBadge), args.WillUpdateBadge ? bool.TrueString : bool.FalseString },
					{ nameof(args.IsGhost), args.IsGhost ? bool.TrueString : bool.FalseString },
					{ nameof(args.State), args.State }
				};

				return query as T;
			}
			else
			{
				return null;
			}
		}
	}
}
