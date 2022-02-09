using UnityEngine;

namespace Game.UI
{
	public class SpinningProgress : MonoBehaviour
	{
		public float _speed = 20f;

		// Update is called once per frame
		private void Update()
		{
			transform.Rotate(0, 0, _speed * Time.deltaTime);
		}
	}
}
