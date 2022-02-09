using System;

using DG.Tweening;

using UnityEngine;

namespace Game.Racer.Modules
{
	public class SlideModule : BaseModule
	{
		[SerializeField]
		private float _rubDeceleration = 200f;

		[SerializeField]
		private GameObject _rubExplosionPrefab;
		private Action _onStop;
		private ParticleSystem _rubExplosion;

		public override void Enable()
		{
			base.Enable();
			_rigidbody.constraints = RigidbodyConstraints.None;
			Controller.gameObject.layer = 0;
		}

		public void RegisterOnStop(Action onStop)
		{
			_onStop = onStop;
		}

		public void RubOnGround(Transform hull)
		{
			// compute the duration
			float duration = Controller.VelocityMeter.Velocity.z / _rubDeceleration;

			// reduce speed to 0
			DOTween.To(() => Controller.VelocityMeter.VelocityWorld, x => _rigidbody.velocity = x, Vector3.zero, duration);

			// add pitch
			hull.DOLocalRotate(new Vector3(hull.localEulerAngles.x + 50, hull.localEulerAngles.y, hull.localEulerAngles.z), duration);

			// add explosion on ground
			GameObject obj = GameObject.Instantiate(_rubExplosionPrefab);
			obj.transform.SetParent(Controller.transform);
			_rubExplosion = obj.GetComponent<ParticleSystem>();
			_rubExplosion.Play();
		}

		public override void ModuleUpdate()
		{
			_rigidbody.AddForce(Vector3.down * 100f);
			if (Controller.VelocityMeter.Velocity.z < 1f && _onStop != null)
			{
				_onStop();
				_onStop = null;
				_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				if (_rubExplosion == null)
				{
					Debug.LogWarning("_rubExplosion is not set");
				}
				else
				{
					GameObject.Destroy(_rubExplosion.gameObject);
				}
				base.Disable();
			}
		}
	}
}
