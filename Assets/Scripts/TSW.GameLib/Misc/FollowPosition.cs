using UnityEngine;

public class FollowPosition : MonoBehaviour
{
	public Transform _target;

	// Update is called once per frame
	private void Update()
	{
		transform.position = _target.position;
	}
}
