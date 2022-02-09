using UnityEngine;

namespace Game
{
	public class DemoObjectActivator : MonoBehaviour
	{
		[SerializeField]
		private bool _isDemoObject;

		[SerializeField]
		private GameObject _alternative;

		public GameObject Alternative => _alternative;

		private void Awake()
		{
			gameObject.SetActive(_isDemoObject == Config.GetAsset().IsDemo);
		}
	}
}