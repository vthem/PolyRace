using UnityEngine;
using UnityEngine.Audio;

namespace TSW.Audio
{
	public class MixerExposedValueModifier : FloatModifier
	{
		public enum ExposedType
		{
			Linear,
			Logarithmic
		}

		[SerializeField]
		private AudioMixer _mixer;

		[SerializeField]
		private string _exposedValueName;

		[SerializeField]
		private ExposedType _exposedType;

		protected override void OnStart()
		{
			float value;
			if (!_mixer.GetFloat(_exposedValueName, out value))
			{
				Debug.LogWarning("Could not find exposed value " + _exposedValueName + " from mixer " + _mixer.name);
			}
		}

		protected override void UpdateValue(float value)
		{
			switch (_exposedType)
			{
				case ExposedType.Linear:
					_mixer.SetFloat(_exposedValueName, value);
					break;
				case ExposedType.Logarithmic:
					_mixer.SetFloat(_exposedValueName, Convert.ToDecibel(value));
					break;
			}
		}

		public override string ToString()
		{
			return string.Format("[MixerExposedValueModifier] Exposed parameter: " + _exposedValueName);
		}
	}
}
