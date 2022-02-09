using System.Collections;

using UnityEngine;

namespace Game
{
	public class DestroyAfterTime : MonoBehaviour
	{

		public float waitTime;

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(waitTime);
			GameObject.Destroy(gameObject);
		}

	}
}
