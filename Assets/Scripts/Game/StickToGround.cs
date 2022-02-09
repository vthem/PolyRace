using UnityEngine;

namespace Game
{
	public class StickToGround : MonoBehaviour
	{
		[SerializeField]
		private LayerMask _groundMask;

		[SerializeField]
		private float _offset = 0.1f;

		[SerializeField]
		private float _period = 0.2f;

		private void Start()
		{
			InvokeRepeating("StickUpdate", 0f, _period);
		}

		// Update is called once per frame
		private void StickUpdate()
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position + Vector3.up * 50f, -Vector3.up, out hit, 100f, _groundMask))
			{
				Vector3 currentPosition = transform.position;
				currentPosition.y = hit.point.y + _offset;
				transform.position = currentPosition;
			}
		}
	}
}
