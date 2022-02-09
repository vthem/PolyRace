using System.Collections;

using UnityEngine;

namespace TSW
{
	public static class DedicatedCoroutineProvider
	{
		public static DedicatedCoroutine NewRoutine(IEnumerator coroutine, string name, bool autoDestroy = false)
		{
			GameObject obj = new GameObject(name);
			DedicatedCoroutine script = obj.AddComponent<DedicatedCoroutine>();
			if (autoDestroy)
			{
				script.HandleCoroutine(coroutine);
			}
			else
			{
				script.StartCoroutine(coroutine);
			}
			return script;
		}
	}

	public class DedicatedCoroutine : MonoBehaviour
	{
		public void HandleCoroutine(IEnumerator coroutine)
		{
			StartCoroutine(InnerCoroutine(coroutine));
		}

		private IEnumerator InnerCoroutine(IEnumerator coroutine)
		{
			yield return StartCoroutine(coroutine);
			GameObject.Destroy(gameObject);
		}
	}
}
