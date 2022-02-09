using UnityEngine;

namespace Game.Racer.Modules
{
	public class ColorModule : BaseModule
	{
		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private int _colorIndex;

		public void SetColor(Material material)
		{
			Material[] m = _renderer.sharedMaterials;
			m[_colorIndex] = material;
			_renderer.sharedMaterials = m;
		}

		public Material GetColor()
		{
			return _renderer.sharedMaterials[_colorIndex];
		}
	}
}
