using System.Collections.Generic;

namespace TSW
{
	public static class AppData
	{
		private static readonly Dictionary<string, object> _datas = new Dictionary<string, object>();

		public static void Store(string name, object obj)
		{
			_datas[name] = obj;
		}

		public static T Get<T>(string name) where T : class
		{
			object obj;
			if (_datas.TryGetValue(name, out obj))
			{
				return obj as T;
			}
			return null;
		}
	}
}
