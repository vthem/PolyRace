using TSW.Noise;

using UnityEngine;

namespace LevelGen
{
	[System.Serializable]
	public class ColorBlender
	{
		[SerializeField]
		private Color _color;

		[SerializeField]
		private Source _noise;

		[SerializeField]
		[Range(0f, 1f)]
		private float _attenuation;

		[SerializeField]
		private float _minHeight = 0f;

		[SerializeField]
		private float _maxHeight = 1000f;

		public Color32 Blend(Color32 color, Vector3 position)
		{
			if ((_minHeight > 0f && position.y < _minHeight)
				|| (_maxHeight > 0f && position.y > _maxHeight))
			{
				return color;
			}
			float noise = _noise.GetFloat(position);
			return Color32.Lerp(color, _color, Mathf.Clamp01(noise - _attenuation));
		}
	}
}
