using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class InRaceNotification : MonoBehaviour
	{
		public delegate string GetTextDelegate();

		[SerializeField]
		private Text _text;

		[SerializeField]
		private CanvasGroup _animateGroup;

		public GetTextDelegate GetText { get; set; }

		private void Start()
		{
			_animateGroup.alpha = 0f;
			DOTween.To(() => _animateGroup.alpha, (a) => _animateGroup.alpha = a, 1f, .5f).SetEase(Ease.Flash, 5f, 0f);
		}

		private void Update()
		{
			if (GetText != null)
			{
				_text.text = GetText();
			}
		}
	}
}
