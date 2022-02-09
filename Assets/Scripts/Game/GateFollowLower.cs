using UnityEngine;

namespace Game
{
	public class GateFollowLower : MonoBehaviour
	{
		public Transform _lower;
		public float _followSpeed = 1f;

		// Update is called once per frame
		private void Update()
		{
			transform.position = Vector3.Lerp(transform.position, _lower.position, Time.deltaTime * _followSpeed);
		}
	}
}
