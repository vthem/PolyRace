using UnityEngine;

namespace LevelGen
{
	public class MeshCleaner : MonoBehaviour
	{
		private void OnDestroy()
		{
			Destroy(GetComponent<MeshFilter>().sharedMesh);
		}
	}
}
