using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RawImage))]
	public class CoverCanvas : UIBehaviour
	{
		private void Update()
		{
			UpdatePositionAndSize();
		}

		private void UpdatePositionAndSize()
		{
			RawImage image = GetComponent<RawImage>();
			if (image == null)
			{
				return;
			}
			if (image.canvas != null)
			{
				RectTransform canvasRect = image.canvas.transform as RectTransform;
				RectTransform rect = transform as RectTransform;
				rect.position = canvasRect.position;
				rect.sizeDelta = canvasRect.sizeDelta;
			}
		}
	}
}
