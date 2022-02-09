using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class TextFlickeringColor : MonoBehaviour
	{
		[SerializeField]
		private float _onDuration;

		[SerializeField]
		private Color _onColor;

		[SerializeField]
		private float _period;
		private Text _text;
		private Color _default;

		// Use this for initialization
		private void Start()
		{
			_text = GetComponent<Text>();
			_default = _text.color;

		}

		public void StartFlickering()
		{
			StartCoroutine(Flickering());
		}

		public void StopFlickering()
		{
			if (_text != null)
			{
				StopAllCoroutines();
				_text.color = _default;
			}
		}

		private IEnumerator Flickering()
		{
			while (true)
			{
				_text.color = _default;
				yield return new WaitForSeconds(_period - _onDuration);
				_text.color = _onColor;
				yield return new WaitForSeconds(_onDuration);
			}
		}
	}
}