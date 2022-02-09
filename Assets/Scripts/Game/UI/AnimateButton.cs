using DG.Tweening;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public class AnimateButton : MonoBehaviour, ISelectHandler, IDeselectHandler
	{
		[SerializeField]
		private RectTransform _text;

		[SerializeField]
		private float _duration = .5f;

		[SerializeField]
		private float _strength = 1.15f;

		[SerializeField]
		private int _vibrato = 10;

		public void OnSelect(BaseEventData eventData)
		{
			Animate();
		}

		public void OnDeselect(BaseEventData eventData)
		{
			_text.DOKill();
			if (_text == null)
			{
				Debug.LogWarning("not set:" + name);
			}
			_text.localScale = Vector3.one;
		}

		public void Animate()
		{
			_text.DOKill();
			_text.DOPunchScale(Vector3.one * _strength, _duration, _vibrato);
		}
	}
}
