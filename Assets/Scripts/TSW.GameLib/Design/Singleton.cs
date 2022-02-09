namespace TSW.Design
{
	public class Singleton<T> where T : new()
	{
		private static T _instance;

		protected Singleton()
		{
			SingletonCreate();
		}

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new T();
				}
				return _instance;
			}
		}

		protected virtual void SingletonCreate()
		{

		}
	}
}