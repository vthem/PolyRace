using UnityEngine;

namespace TSW
{
	public class TransformBackup
	{
		protected Vector3 _position;
		protected Quaternion _rotation;

		public void Backup(Transform t)
		{
			_position = t.position;
			_rotation = t.rotation;
		}

		public void Restore(Transform t)
		{
			t.position = _position;
			t.rotation = _rotation;
		}
	}
}