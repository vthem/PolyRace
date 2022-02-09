using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class OptionsVolume : MonoBehaviour
	{
		[SerializeField]
		private Text _volumeValue;

		[SerializeField]
		private Game.Options.AudioMixerType _mixerType;

		public void OnIncreaseButton()
		{
			float v = Mathf.Min(1f, Audio.Master.Instance.GetVolume(_mixerType) + .1f);
			Audio.Master.Instance.SetVolume(_mixerType, v);
			Game.Options.SaveVolume(_mixerType, v);
		}

		public void OnDecreaseButton()
		{
			float v = Mathf.Max(0f, Audio.Master.Instance.GetVolume(_mixerType) - .1f);
			Audio.Master.Instance.SetVolume(_mixerType, v);
			Game.Options.SaveVolume(_mixerType, v);
		}

		private void Update()
		{
			_volumeValue.text = (System.Math.Round(Audio.Master.Instance.GetVolume(_mixerType), 1) * 100f).ToString() + "%";
		}
	}
}
