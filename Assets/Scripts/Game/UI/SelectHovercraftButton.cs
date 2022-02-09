using TSW.Messaging;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	[RequireComponent(typeof(Button))]
	public class SelectHovercraftButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private int _hoverId;

		[SerializeField]
		private HovercraftRadar _radar;

		[SerializeField]
		private SelectedButton _selectedButton;

		private void Awake()
		{
			Dispatcher.AddHandler<Race.Setup.UpdatedEvent>(OnRacerSelectionUpdate);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (_hoverId != Race.Setup.Next.RacerDynamicProperties.Properties.Id)
			{
				_radar.DisplaySecondary(_hoverId);
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (_hoverId != Race.Setup.Next.RacerDynamicProperties.Properties.Id)
			{
				_radar.DisplaySecondary(_hoverId);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_radar.DisplaySecondary(-1);
		}

		public void OnDeselect(BaseEventData eventData)
		{
			_radar.DisplaySecondary(-1);
		}

		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(() =>
			{
				Race.Setup.Next.UpdateRacerDynamicProperties(Racer.DynamicProperties.NewFromRacerId(_hoverId));
				_radar.DisplayPrimary(_hoverId);
			});
		}

		public void OnRacerSelectionUpdate(Race.Setup.UpdatedEvent evt)
		{
			UpdateSelectedButton(evt.Value1.RacerDynamicProperties.Properties.Id);
		}

		private void UpdateSelectedButton(int id)
		{
			if (id == _hoverId)
			{
				_selectedButton.SetAsSelected();
			}
			else
			{
				_selectedButton.SetAsNotSelected();
			}
		}

		private void OnEnable()
		{
			if (Race.Setup.Next != null)
			{
				UpdateSelectedButton(Race.Setup.Next.RacerDynamicProperties.Properties.Id);
			}
		}
	}
}