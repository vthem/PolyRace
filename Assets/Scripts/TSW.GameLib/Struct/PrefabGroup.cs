using System.Collections.Generic;

using UnityEngine;

namespace TSW.Struct
{
	public class PrefabGroup : ScriptableObject
	{
		[SerializeField]
		private List<GameObject> _objects;

		public List<GameObject> Objects => _objects;
	}
}
