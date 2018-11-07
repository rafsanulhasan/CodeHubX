using ArgumentNullException = System.ArgumentNullException;

namespace CodeHubX.Helpers
{
	public static class ParseHelper
	{
		public static bool TryParse<TTargetType>(this object sourceType, out TTargetType targetType)
		    where TTargetType : class
		{
			if (sourceType is null)
			{
				throw new ArgumentNullException(nameof(sourceType));
			}
			var result = false;
			targetType = null;

			if (sourceType is int intSourceType && targetType is int)
			{
				targetType = intSourceType as TTargetType;
				result = true;
			}
			else if (sourceType is long longSourceType && targetType is long)
			{
				targetType = longSourceType as TTargetType;
				result = true;
			}
			else if (sourceType is bool boolSourceType && targetType is bool)
			{
				targetType = boolSourceType as TTargetType;
				result = true;
			}
			else if (sourceType is string strSourceType && targetType is string)
			{
				targetType = strSourceType as TTargetType;
				result = true;
			}
			else if (sourceType is object objSourceType && targetType is object)
			{
				targetType = objSourceType as TTargetType;
				result = true;
			}

			return result;
		}
	}
}
