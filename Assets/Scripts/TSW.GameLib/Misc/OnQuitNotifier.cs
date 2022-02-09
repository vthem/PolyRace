namespace TSW
{
	public class OnQuitNotifier : TSW.Design.USingleton<OnQuitNotifier>
	{
		public event System.Action OnQuit;

		protected override void SingletonCreate()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void OnApplicationQuit()
		{
			if (OnQuit != null)
			{
				OnQuit();
			}
		}
	}
}
