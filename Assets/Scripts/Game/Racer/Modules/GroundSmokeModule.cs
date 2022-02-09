using UnityEngine;

namespace Game.Racer.Modules
{
	public class GroundSmokeModule : BaseModule
	{
		private ParticleSystem _smoke;

		[SerializeField]
		private Transform _smokeContainer;

		public override void Disable()
		{
			base.Disable();
			if (_smoke != null)
			{
				_smoke.Stop();
			}
		}

		public void Enable(GameObject smokePrefab)
		{
			base.Enable();
			GameObject obj = GameObject.Instantiate(smokePrefab, Controller.transform.position, Quaternion.identity);
			if (obj != null)
			{
				obj.transform.SetParent(_smokeContainer);
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localRotation = Quaternion.identity;
				obj.transform.localScale = Vector3.one;
				_smoke = obj.GetComponent<ParticleSystem>();
				_smoke.Play();
			}
		}
	}
}
