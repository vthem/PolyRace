using System;
using System.Collections;
using System.Text;

using DG.Tweening;

using UnityEngine;

namespace Game.UI
{
	public class HUD : UIElement
	{
		[SerializeField]
		private UnityEngine.UI.Text _chronometer;

		[SerializeField]
		private UnityEngine.UI.Text _speedometer;

		[SerializeField]
		private UnityEngine.UI.Text _distance;

		[SerializeField]
		private UnityEngine.UI.Image _shieldBar;

		[SerializeField]
		private UnityEngine.UI.Image _reverseShieldBar;

		[SerializeField]
		private UnityEngine.UI.Image _speedBar;

		[SerializeField]
		private UnityEngine.UI.Image _reverseSpeedBar;

		[SerializeField]
		private UnityEngine.UI.Text _shieldValueText;

		[SerializeField]
		private CanvasGroup _shieldGroup;

		[SerializeField]
		private UnityEngine.UI.Text _speedUnitText;
		private Func<string> _getScore;
		private Func<string> _getTime;
		private Func<string> _getSpeed;
		public Func<float> GetNormalizedSpeed { get; private set; }

		private Func<float> _getNormalizedEnergy;
		private readonly Camera.Handle _cameraHandle;
		private StringBuilder _shieldValueBuilder;
		private Tweener _shieldTweener;
		private bool _inShieldTween;
		private float _lastShieldValue;

		public override void Initialize()
		{
			base.Initialize();
			enabled = false;
			_shieldValueBuilder = new StringBuilder();
		}

		private void Update()
		{
			_speedometer.text = _getSpeed();
			_shieldBar.fillAmount = 0.05f + _getNormalizedEnergy() * 0.90f;
			_speedBar.fillAmount = 0.05f + GetNormalizedSpeed() * 0.90f;
			_reverseSpeedBar.fillAmount = 1 - _speedBar.fillAmount;
			_reverseShieldBar.fillAmount = 1 - _shieldBar.fillAmount;
			_distance.text = _getScore();
			_chronometer.text = _getTime();

			_shieldValueBuilder.Length = 0;
			_shieldValueBuilder.AppendFormat("{0:D3}%", Mathf.RoundToInt(_getNormalizedEnergy() * 100f));
			_shieldValueText.text = _shieldValueBuilder.ToString();

			if (!_inShieldTween)
			{
				_shieldGroup.alpha = 1f - _getNormalizedEnergy() * 0.8f;
			}
			if (_getNormalizedEnergy() < _lastShieldValue)
			{
				if (_shieldTweener != null)
				{
					_shieldTweener.Kill();
				}
				_shieldGroup.alpha = 0.2f;
				_inShieldTween = true;
				_shieldTweener = DOTween.To(() => _shieldGroup.alpha, (a) => _shieldGroup.alpha = a, 1f, .5f).SetEase(Ease.Flash, 5f, 0f).OnComplete(
					() => _inShieldTween = false
				);
			}

			_lastShieldValue = _getNormalizedEnergy();
		}

		protected override IEnumerator OnDisplay()
		{
			enabled = true;
			_lastShieldValue = 1f;
			_inShieldTween = false;
			_speedUnitText.text = SpeedUnit.UnitString(Player.PlayerManager.Instance.GetSpeedUnit());
			yield return null;
		}

		protected override IEnumerator OnHide()
		{
			enabled = false;
			yield return null;
		}

		public void SetDataMethod(Func<string> score,
								  Func<string> time,
								  Func<string> speed,
								  Func<float> normalizedSpeed,
								  Func<float> normalizedEnergy)
		{
			_getScore = score;
			_getTime = time;
			_getSpeed = speed;
			GetNormalizedSpeed = normalizedSpeed;
			_getNormalizedEnergy = normalizedEnergy;
		}
	}
}