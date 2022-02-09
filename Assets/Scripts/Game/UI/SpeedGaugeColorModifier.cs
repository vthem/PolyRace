using TSW.Messaging;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

using BDEvent = TSW.Messaging.Event;

namespace Game.UI
{
	public class SpeedGaugeColorModifier : MonoBehaviour
	{
		[SerializeField]
		private Color _defaultColor;

		[SerializeField]
		private Color _highlightColor;

		[SerializeField]
		private Graphic _targetGraphic;

		[SerializeField]
		private float _tweenDuration = .2f;
		private Tween _colorTweener;

		private void Awake()
		{
			Dispatcher.AddHandler(typeof(Racer.EnergyController.BoostActivatedEvent), OnBoostActivated);
			Dispatcher.AddHandler(typeof(Racer.EnergyController.BoostDeactivatedEvent), OnBoostDeactivated);
		}

		public void OnBoostActivated(BDEvent evt)
		{
			TweenToHighlight();
		}

		public void OnBoostDeactivated(BDEvent evt)
		{
			TweenToDefault();
		}

		private void OnEnable()
		{
			SetDefaultColor();
		}

		private void SetDefaultColor()
		{
			_targetGraphic.color = _defaultColor;
		}

		private void SetHighlightColor()
		{
			_targetGraphic.color = _highlightColor;
		}

		private void TweenToDefault()
		{
			KillTweener();
			_colorTweener = DOTween.To(() => _targetGraphic.color, (c) => _targetGraphic.color = c, _defaultColor, _tweenDuration);
		}

		private void TweenToHighlight()
		{
			KillTweener();
			_colorTweener = DOTween.To(() => _targetGraphic.color, (c) => _targetGraphic.color = c, _highlightColor, _tweenDuration);
		}

		private void KillTweener()
		{
			if (_colorTweener != null)
			{
				_colorTweener.Kill();
			}
		}
	}
}
