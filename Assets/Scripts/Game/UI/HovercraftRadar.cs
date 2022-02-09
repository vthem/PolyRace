using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class HovercraftRadar : MonoBehaviour
	{
		[SerializeField]
		private Color _primaryColor;

		[SerializeField]
		private Color _secondaryColor;

		[SerializeField]
		private Sprite[] _hoverRadars;

		[SerializeField]
		private Image _primaryImage;

		[SerializeField]
		private Image _secondaryImage;

		private void OnEnable()
		{
			if (Race.Setup.Next != null)
			{
				DisplayPrimary(Race.Setup.Next.RacerDynamicProperties.Properties.Id);
			}
		}

		public void DisplayPrimary(int hoverId)
		{
			if (hoverId < 0)
			{
				_primaryImage.gameObject.SetActive(false);
				return;
			}
			_primaryImage.sprite = _hoverRadars[hoverId];
			_primaryImage.color = _primaryColor;
			_primaryImage.gameObject.SetActive(true);
		}

		public void DisplaySecondary(int hoverId)
		{
			if (hoverId < 0)
			{
				_secondaryImage.gameObject.SetActive(false);
				return;
			}
			_secondaryImage.sprite = _hoverRadars[hoverId];
			_secondaryImage.color = _secondaryColor;
			_secondaryImage.gameObject.SetActive(true);
		}
	}
}