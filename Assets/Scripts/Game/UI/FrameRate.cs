using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class FrameRate : MonoBehaviour
	{
		private Text _text;
		private readonly TSW.FramPerSecond _framePerSecond = new TSW.FramPerSecond(.5f);

		// Use this for initialization
		private void Start()
		{
			_text = GetComponent<Text>();
		}

		// Update is called once per frame
		private void Update()
		{
			_framePerSecond.Update();
			_text.text = Mathf.RoundToInt(_framePerSecond.FPS).ToString();
		}
	}
}
