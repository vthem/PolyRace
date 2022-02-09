using System.Collections.Generic;

using UnityEngine;

namespace LevelGen
{
	public class ColorProfile : ScriptableObject
	{
		[SerializeField]
		private Color _defaultColor;

		[SerializeField]
		private List<ColorBlender> _colorBlenders;

		public Color32 BlendColor(Vector3 position)
		{
			Color32 color = _defaultColor;
			for (int cbi = 0; cbi < _colorBlenders.Count; ++cbi)
			{
				color = _colorBlenders[cbi].Blend(color, position);
			}
			return color;
		}


	}
}
