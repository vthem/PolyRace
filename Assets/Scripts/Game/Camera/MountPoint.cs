using UnityEngine;

namespace Game.Camera
{
	public class MountPoint : MonoBehaviour
	{
		[SerializeField]
		private Transform[] _points;

		public Transform GetMountPoint(int index)
		{
			return _points[index % _points.Length];
		}
	}
}
