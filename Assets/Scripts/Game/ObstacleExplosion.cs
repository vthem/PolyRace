using UnityEngine;

namespace Game
{
	public class ObstacleExplosion : MonoBehaviour
	{
		[SerializeField]
		private Mesh _lowMesh = null;
		public void Explode(Vector3 impactForward, Vector3 impactUpward, float velocity, bool removeObject)
		{
			if (!_lowMesh)
			{
				Debug.LogWarning($"_lowMesh is not set on {gameObject.name}");
				return;
			}
			GameObject explodedObj = Instantiate(gameObject);
			Destroy(explodedObj.GetComponent<MeshCollider>());
			Destroy(explodedObj.GetComponent<ObstacleExplosion>());
			explodedObj.transform.position = transform.position;
			explodedObj.transform.rotation = transform.rotation;
			explodedObj.transform.localScale = transform.localScale;
			explodedObj.GetComponent<MeshFilter>().mesh = _lowMesh;

			TSW.MeshExplosion exp = explodedObj.GetComponent<TSW.MeshExplosion>();
			exp.spreadforce = velocity;
			exp.spreadDirection = impactForward;
			exp.destroyOnEnd = true;
			exp.Initialize();

			if (removeObject)
			{
				transform.gameObject.SetActive(false);
				LevelGen.LevelBuilder.Instance.Level.AddReUseObject(transform.gameObject);
			}
		}
	}
}
