using UnityEngine;

namespace Game.Bonus
{
	public class TorusAnimation : MonoBehaviour
	{
		public Vector3 _rotationSpeed;

		// Update is called once per frame
		private void Update()
		{
			transform.Rotate(Time.deltaTime * _rotationSpeed);
		}
	}
}
