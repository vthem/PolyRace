using System.Collections;

using UnityEngine;

namespace Game.Bonus
{
	public class Attraction : MonoBehaviour
	{
		[SerializeField]
		private Controller _bonus;

		[SerializeField]
		private float _speed = 10f;

		[SerializeField]
		private float _minAttractionAngle = 20;

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.tag != "PlayerRacer")
			{
				return;
			}
			StartCoroutine(FollowTarget(other.transform));
		}

		private IEnumerator FollowTarget(Transform target)
		{
			Transform bonusTransform = _bonus.transform;
			while ((target.position - bonusTransform.position).magnitude > 5f)
			{
				yield return null;
				float distance = Vector3.Dot(target.forward, (bonusTransform.transform.position - target.position));
				Vector3 targetPoint = target.position + target.forward * distance;
				Vector3 direction = (targetPoint - bonusTransform.position).normalized;
				float angle = Mathf.Abs(Vector3.Angle(target.forward, (bonusTransform.position - target.position).normalized));
				if (angle > 1f && angle < _minAttractionAngle)
				{
					bonusTransform.transform.Translate(direction * Time.deltaTime * _speed, Space.World);
					RaycastHit hit;
					if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, _bonus.GroundLayer))
					{
						float y = hit.point.y + _bonus.GroundOffset;
						bonusTransform.position = new Vector3(bonusTransform.position.x, y, bonusTransform.position.z);
					}
				}
			}
		}
	}
}