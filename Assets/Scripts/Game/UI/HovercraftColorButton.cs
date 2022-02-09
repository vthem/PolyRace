using TSW.Messaging;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class HovercraftColorButton : MonoBehaviour
	{
		[SerializeField]
		private Material _colorMaterial;

		[SerializeField]
		private RawImage _imageColorPreview;

		[SerializeField]
		private SelectedButton _selectedButton;

		[SerializeField]
		private Racer.ColorId _colorId;

		private void Awake()
		{
			Dispatcher.AddHandler<Race.Setup.UpdatedEvent>(OnColorUpdateEvent);
		}

		private void Start()
		{
			_imageColorPreview.color = _colorMaterial.GetColor("_Color");
		}

		private void OnEnable()
		{
			if (Race.Setup.Next != null)
			{
				UpdateSelectedButton(Race.Setup.Next.RacerColor);
			}
		}

		public void OnSetColorButton()
		{
			if (Race.Setup.Next != null)
			{
				Race.Setup.Next.UpdateRacerColor(_colorMaterial, _colorId);
			}
		}

		public void OnColorUpdateEvent(Race.Setup.UpdatedEvent evt)
		{
			UpdateSelectedButton(evt.Value1.RacerColor);
		}

		private void UpdateSelectedButton(Material racerColor)
		{
			if (_colorMaterial == racerColor)
			{
				_selectedButton.SetAsSelected();
			}
			else
			{
				_selectedButton.SetAsNotSelected();
			}
		}
	}
}