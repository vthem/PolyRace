using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	[RequireComponent(typeof(AnimateButton))]
	public class ButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler
	{
		[SerializeField]
		private Text _text;

		[SerializeField]
		private Color _highlightColor;

		[SerializeField]
		private float _blinkPeriod = 2f;
		private Color _defaultColor;
		private bool _initialized = false;
		private AnimateButton _animator;

		private void Start()
		{
			InitializeOnce();
		}

		private void InitializeOnce()
		{
			if (!_initialized)
			{
				_initialized = true;
				_defaultColor = _text.color;
				_animator = GetComponent<AnimateButton>();
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			InitializeOnce();
			_text.color = _highlightColor;
			StartCoroutine(Animate());
		}

		public void OnDeselect(BaseEventData eventData)
		{
			StopAllCoroutines();
			if (_initialized)
			{
				_text.color = _defaultColor;
			}
		}

		private void OnDisable()
		{
			StopAllCoroutines();
			if (_initialized)
			{
				_text.color = _defaultColor;
			}
		}

		private IEnumerator Animate()
		{
			while (true)
			{
				_animator.Animate();
				yield return new WaitForSeconds(_blinkPeriod);
			}
		}
	}
}
