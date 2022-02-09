using UnityEngine;

namespace Game
{
	public class PodiumEnabler : MonoBehaviour
	{
		public GameObject _podium;

		private void OnEnable()
		{
			_podium.SetActive(true);
		}

		private void OnDisable()
		{
			if (_podium != null)
			{
				_podium.SetActive(false);
			}
		}
	}
}
