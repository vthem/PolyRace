using UnityEngine;

namespace TSW
{
	public class OnDestroyNotifier : MonoBehaviour
	{
		public delegate void OnDestroyDelegate(Transform t);

		private event OnDestroyDelegate OnDestroyEvent;

		public static void RegisterOnDestroy(Transform transform, OnDestroyDelegate onDestroyDelegate)
		{
			transform.gameObject.AddComponent<OnDestroyNotifier>().OnDestroyEvent += onDestroyDelegate;
		}

		public static void RegisterOnDestroy(Component component, OnDestroyDelegate onDestroyDelegate)
		{
			component.gameObject.AddComponent<OnDestroyNotifier>().OnDestroyEvent += onDestroyDelegate;
		}

		public static void UnregisterOnDestroy(Transform transform, OnDestroyDelegate onDestroyDelegate)
		{
			if (transform != null)
			{
				OnDestroyNotifier notifier = transform.GetComponent<OnDestroyNotifier>();
				if (null != notifier)
				{
					notifier.OnDestroyEvent -= onDestroyDelegate;
				}
			}
		}

		public static void UnregisterOnDestroy(Component component, OnDestroyDelegate onDestroyDelegate)
		{
			if (component != null)
			{
				OnDestroyNotifier notifier = component.GetComponent<OnDestroyNotifier>();
				if (null != notifier)
				{
					notifier.OnDestroyEvent -= onDestroyDelegate;
				}
			}
		}

		private void OnDestroy()
		{
			if (OnDestroyEvent != null)
			{
				try
				{
					OnDestroyEvent(transform);
				}
				catch (System.Exception ex)
				{
					Debug.Log("Destroy [" + name + "] Notify fail:" + ex.Message);
					Debug.Log(ex.StackTrace);
				}
			}
		}
	}
}
