using TSW.Messaging;

using DG.Tweening;

using UnityEngine;

namespace Game.Bonus
{
	public class Controller : MonoBehaviour
	{
		public class BonusEvent : Event<BonusType> { }

		[SerializeField]
		private Light _light;

		[SerializeField]
		private float _lightMinRange = 100f;

		[SerializeField]
		private float _lightMinIntensity = .4f;

		[SerializeField]
		private LayerMask _groundLayer;
		public LayerMask GroundLayer => _groundLayer;

		[SerializeField]
		private float _groundOffset = 5f;
		public float GroundOffset => _groundOffset;

		[SerializeField]
		private ParticleSystem _skyExplosion;

		[SerializeField]
		private float _particleSystemMinSpeed = 20f;

		[SerializeField]
		private float _particleSystemMinEmit = 100f;

		[SerializeField]
		private float _particleSystemMinStartSize = 10f;

		[SerializeField]
		private float _dropDuration = 1f;

		[SerializeField]
		private GameObject[] _racerImpactExploder;

		[SerializeField]
		private float _distanceBonusHovercraftEffect = 4f;

		[SerializeField]
		private GameObject _rayBeam;

		private void Start()
		{
			Audio.SoundFx.Instance.Play("BonusExplode3D", transform, Audio.MixerOption.Default);
			// start the animation
			DOTween.To(() => _light.range, (v) => _light.range = v, _lightMinRange, _dropDuration);
			DOTween.To(() => _light.intensity, (v) => _light.intensity = v, _lightMinIntensity, _dropDuration);
			DOTween.To(() => _skyExplosion.GetStartSpeed(), (v) => _skyExplosion.SetStartSpeed(v), _particleSystemMinSpeed, _dropDuration);
			DOTween.To(() => _skyExplosion.GetEmissionRate(), (v) => _skyExplosion.SetEmissionRate(v), _particleSystemMinEmit, _dropDuration);
			DOTween.To(() => _skyExplosion.GetStartSize(), (v) => _skyExplosion.SetStartSize(v), _particleSystemMinStartSize, _dropDuration);
			RaycastHit hit;
			if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, _groundLayer))
			{
				Vector3 pos = hit.point;
				pos.y += _groundOffset;
				transform.DOMove(pos, _dropDuration);
			}
		}

		private void ExplodeUp()
		{
			FragmentBonus(Vector3.up, Vector3.up, 5f);
			GetComponent<Collider>().enabled = false;
			gameObject.SetActive(false);
			_rayBeam.SetActive(false);
			GameObject.Destroy(gameObject, 5f);
		}

		public static void MoveToSkyAllBonuses()
		{
			GameObject[] bonuses = GameObject.FindGameObjectsWithTag("Bonus");
			for (int i = 0; i < bonuses.Length; ++i)
			{
				Controller ctl = bonuses[i].GetComponent<Controller>();
				if (ctl != null)
				{
					ctl.ExplodeUp();
				}
				else
				{
					Debug.Log("cannot get controller from bonus " + bonuses[i].name);
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.tag != "PlayerRacer")
			{
				return;
			}

			// one time collide
			GetComponent<Collider>().enabled = false;

			gameObject.SetActive(false);
			Racer.Controller racerController = other.transform.GetComponent<Racer.Controller>();
			FragmentBonus(racerController.HullModule._terrainRollPitch.forward, racerController.HullModule._terrainRollPitch.up, racerController.VelocityMeter.Velocity.z);
			racerController.BonusModule.Duration = _distanceBonusHovercraftEffect;
			racerController.BonusModule.Enable();

			// send event with duration of the bonus
			Dispatcher.FireEvent(new BonusEvent().SetValue1(BonusType.InverseTimer));
		}

		private void FragmentBonus(Vector3 forward, Vector3 up, float velocity)
		{
			Vector3 scale = Vector3.zero;
			bool first = true;
			foreach (GameObject obj in _racerImpactExploder)
			{
				if (first)
				{
					scale = obj.transform.localScale;
					first = false;
				}
				GameObject explodedObj = Instantiate(obj);
				for (int i = 0; i < explodedObj.transform.childCount; ++i)
				{
					Destroy(explodedObj.transform.GetChild(i).gameObject);
				}
				explodedObj.transform.position = obj.transform.position;
				explodedObj.transform.rotation = obj.transform.rotation;
				explodedObj.transform.localScale = scale;

				TSW.MeshExplosion exp = explodedObj.GetComponent<TSW.MeshExplosion>();
				exp.spreadforce = velocity;
				exp.spreadDirection = forward;
				exp.destroyOnEnd = true;
				exp.Initialize();

				obj.SetActive(false);
			}
		}

		[ContextMenu("TestFragment")]
		private void TestFragment()
		{
			FragmentBonus(Vector3.up, Vector3.zero, 10f);
		}
	}
}
