using LevelGen;

using UnityEngine;

namespace Game.ParticleEffect
{
	public class ParticleInstantiator : MonoBehaviour
	{
		public enum GroundParticleType
		{
			Gate,
			Air
		};

		public GroundParticleType _type;
		private ParticleSystem _particle;

		private void OnEnable()
		{
			GameObject prefab = GetPrefab();
			if (null == prefab)
			{
				return;
			}
			if (_particle == null)
			{
				CreateParticle(prefab);
			}
			if (_particle.name != prefab.name)
			{
				CreateParticle(prefab);
			}
			if (!_particle.isPlaying)
			{
				_particle.Play();
			}
		}

		private void OnDisable()
		{
			if (_particle != null)
			{
				_particle.Stop();
			}
		}

		private void CreateParticle(GameObject prefab)
		{
			GameObject obj = GameObject.Instantiate(prefab, transform.position, Quaternion.identity);
			obj.transform.SetParent(transform);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;
			obj.name = prefab.name;
			if (_particle != null)
			{
				GameObject.Destroy(_particle.gameObject);
			}
			_particle = obj.GetComponent<ParticleSystem>();
			_particle.Play();
		}

		private GameObject GetPrefab()
		{
			LevelBuilder builder = LevelBuilder.Instance;
			GameObject prefab = null;
			if (builder.Level != null)
			{
				switch (_type)
				{
					case GroundParticleType.Gate:
						prefab = builder.Level.Profile._gateGroundParticlePrefab;
						break;
					case GroundParticleType.Air:
						prefab = builder.Level.Profile._airParticlePrefab;
						break;
				}
			}
			return prefab;
		}
	}
}
