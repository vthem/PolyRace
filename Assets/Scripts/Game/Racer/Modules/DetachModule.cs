using UnityEngine;

namespace Game.Racer.Modules
{
	public class DetachModule : BaseModule
	{
		[SerializeField]
		private GameObject[] _parts;

		public override void Enable()
		{
			base.Enable();
			foreach (GameObject part in _parts)
			{
				part.AddComponent<MeshCollider>().convex = true;
				part.transform.SetParent(null, true);
				part.layer = 0;
				part.AddComponent<Rigidbody>().velocity = Controller.VelocityMeter.VelocityWorld * 0.5f;
				GameObject.Destroy(part.gameObject, 5f);
			}
		}
	}
}
