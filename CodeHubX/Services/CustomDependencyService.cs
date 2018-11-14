using DS = Xamarin.Forms.DependencyService;

namespace CodeHubX.Services
{
	public class CustomDependencyService
	{
		public static void Register<T>(T param)
			where T : class
		{
			DS.Register<T>();
			var t = param.GetType();
			var propInfo = typeof(T).GetProperty(t.Name, t);
			var obj = DS.Get<T>();
			propInfo.SetValue(obj, param);
		}

		public static void Register<T, TParam>(T instance, TParam param)
			where T : class
		{
			DS.Register<T>();
			var t = param.GetType();
			var propInfo = typeof(T).GetProperty(t.Name, t);
			var obj = DS.Get<T>();
			propInfo.SetValue(obj, param);
		}

		public static void Register<T, TImpl, TParam>(TParam param)
			where T : class
			where TImpl : class, T
		{
			DS.Register<T, TImpl>();
			var t = param.GetType();
			var propInfo = typeof(T).GetProperty(t.Name, t);
			var obj = DS.Get<T>();
			propInfo.SetValue(obj, param);
		}

		public static void Register<T, TImpl, TParam>(TImpl instance, TParam param)
			where T : class
			where TImpl : class, T
		{
			DS.Register<T, TImpl>();
			var t = param.GetType();
			var propInfo = typeof(T).GetProperty(t.Name, t);
			var obj = DS.Get<T>();
			propInfo.SetValue(obj, param);
		}
	}
}
