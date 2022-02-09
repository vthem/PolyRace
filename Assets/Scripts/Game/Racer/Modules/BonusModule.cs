using DG.Tweening;

using UnityEngine;

namespace Game.Racer.Modules
{
	public class BonusModule : BaseModule
	{
		[SerializeField]
		private float _minSpeed;

		[SerializeField]
		private float _maxSpeed;

		[SerializeField]
		private float _minNormalizedSpeed = 0.3f;

		[SerializeField]
		private float _duration = 4f;

		public float Duration { get => _duration; set => _duration = value; }

		private ParticleSystem _bonusEffect;
		private float _distance;
		private float _startRate;
		private Tweener _emitTweener;

		protected override void ModuleInit()
		{
			base.ModuleInit();
			_bonusEffect = GetComponent<ParticleSystem>();
			_distance = _bonusEffect.GetStartLifetime() * _bonusEffect.GetStartSpeed();
			_startRate = _bonusEffect.GetEmissionRate();
		}

		public override void Enable()
		{
			base.Enable();
			_bonusEffect.Play();
			_bonusEffect.SetEmissionRate(_startRate);
			if (_emitTweener != null)
			{
				_emitTweener.Kill();
			}
			_emitTweener = DOTween.To(() => _bonusEffect.GetEmissionRate(), (v) => _bonusEffect.SetEmissionRate(v), 0f, _duration).OnComplete(() => Disable());
		}

		public override void ModuleUpdate()
		{
			float normSpeed = Controller.DataModule.NormalizedSpeed;
			if (normSpeed < _minNormalizedSpeed)
			{
				Disable();
				return;
			}
			_bonusEffect.SetStartSpeed(Mathf.Lerp(_minSpeed, _maxSpeed, normSpeed));
			_bonusEffect.SetStartLifetime(_distance / _bonusEffect.GetStartSpeed());
		}

		public override void Disable()
		{
			base.Disable();
			_bonusEffect.Stop();
		}
	}
}