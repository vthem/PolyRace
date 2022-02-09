using UnityEngine;

namespace TSW
{
	namespace Camera
	{
		public class FollowTop : MonoBehaviour
		{
			public Transform _target;
			public float _height;

			// Update is called once per frame
			private void Update()
			{
				if (_target)
				{
					Vector3 pos = _target.position;
					pos.y += _height;
					transform.position = pos;
					transform.rotation = _target.rotation;
					transform.Rotate(90, 0, 0);
				}
			}
		}
	}
}
