using UnityEngine;

namespace TSW
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		private void Awake()
		{
			Debug.Log("awake id:" + gameObject.GetInstanceID());
			GameObject obj = GameObject.Find(gameObject.name);
			if (obj != gameObject)
			{
				Debug.Log("destroy obj:" + gameObject.name + " id:" + gameObject.GetInstanceID());
				GameObject.DestroyImmediate(gameObject);
			}
			else
			{
				GameObject.DontDestroyOnLoad(gameObject);
			}
		}
	}
}
