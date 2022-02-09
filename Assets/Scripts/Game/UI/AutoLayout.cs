using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class AutoLayout : UIBehaviour, ILayoutElement
	{
		[SerializeField]
		private RectTransform _target;

		[SerializeField]
		private float _margin;

		public float minWidth
		{
			get
			{
				// avoid overflow ...
				if (_target == transform)
				{
					Debug.LogWarning("AutoLayout overflow block in " + name);
					return 0f;
				}
				return LayoutUtility.GetPreferredWidth(_target) + _margin;
			}
		}

		public float preferredWidth => 0f;

		public float flexibleWidth => 0f;

		public float minHeight => 0f;

		public float preferredHeight => 0f;

		public float flexibleHeight => 0f;

		public int layoutPriority => 0;

		public void CalculateLayoutInputHorizontal()
		{
		}

		public void CalculateLayoutInputVertical()
		{
		}
	}
}