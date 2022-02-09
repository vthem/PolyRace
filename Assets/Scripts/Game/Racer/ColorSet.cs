using UnityEngine;

namespace Game.Racer
{
	public class ColorSet : ScriptableObject
	{
		[SerializeField]
		private Material[] _materials;

		[SerializeField]
		private Material _default;

		public Material GetMaterial(ColorId id)
		{
			int intId = (int)id;
			if (intId < 0 || intId >= _materials.Length)
			{
				Debug.LogWarning("Material not found in ColorSet id:" + id);
				return _default;
			}
			return _materials[intId];
		}

		public static ColorSet GetAsset()
		{
			return Resources.Load("Materials/Racer/ColorSet") as ColorSet;
		}
	}
}
