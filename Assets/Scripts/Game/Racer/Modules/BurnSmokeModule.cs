using UnityEngine;

namespace Game.Racer.Modules
{
	public class BurnSmokeModule : BaseModule
	{
		[SerializeField]
		private GameObject _smokePrefab;

		public override void Enable()
		{
			base.Enable();
			GameObject smokeObj = GameObject.Instantiate(_smokePrefab);
			smokeObj.transform.SetParent(Controller.transform);
			smokeObj.transform.localPosition = Vector3.zero;
			smokeObj.GetComponent<ParticleSystem>().Play();
		}
	}
}
