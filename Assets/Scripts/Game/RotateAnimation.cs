using UnityEngine;

namespace Game
{
	public class RotateAnimation : MonoBehaviour
	{
		public float _rotateSpeed = 30f;

		private void Update()
		{
			transform.Rotate(new Vector3(0, _rotateSpeed * Time.deltaTime, 0));
		}
	}
}
