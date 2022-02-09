using System.Collections;

using DG.Tweening;

using UnityEngine;

namespace Game.UI
{
	public class ColorModifier : MonoBehaviour
	{
		[System.Serializable]
		public struct TweenColor
		{
			public string _event;
			public Color _fromColor;
			public Color _toColor;
			public float _duration;
			public float _stage;
		}
		[SerializeField]
		private TweenColor[] _tweenColors;
		private UnityEngine.UI.Graphic _graphic;
		private Tween _tween;

		protected void Fade(string eventName)
		{
			if (!gameObject.activeInHierarchy)
			{
				return;
			}
			if (_graphic != null)
			{
				TweenColor tweenColor = new TweenColor();
				foreach (TweenColor c in _tweenColors)
				{
					if (c._event == eventName)
					{
						tweenColor = c;
						break;
					}
				}
				if (gameObject.activeSelf)
				{
					StartCoroutine(WaitTween(tweenColor));
				}
			}
		}

		private IEnumerator WaitTween(TweenColor tweenColor)
		{
			if (_tween != null)
			{
				_tween.Complete();
			}
			_graphic.color = tweenColor._fromColor;
			yield return new WaitForSeconds(tweenColor._stage);
			_tween = _graphic.DOColor(tweenColor._toColor, tweenColor._duration);
		}

		private void Start()
		{
			_graphic = GetComponent<UnityEngine.UI.Graphic>();
		}

		protected virtual void Awake()
		{

		}
	}
}
