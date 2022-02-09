using UnityEngine;

namespace Game.Racer.Modules
{
	public class ExplosionModule : BaseModule
	{
		public void Explode()
		{
			if (enabled)
			{
				Audio.SoundFx.Instance.Play("Crash3D", Controller.transform, Controller.Helper.MixerOption);
				GameObject obj = CommonProperties.InstantiateExplosionPrefab();
				obj.transform.position = Controller.CollisionModule.GetCollisionPoint();
				obj.GetComponent<ParticleSystem>().Play();
			}
		}
	}
}