using UnityEngine;
using UnityEngine.EventSystems;

namespace TSW
{
	public class InputFieldTabSwitcher : MonoBehaviour
	{
		private void Update()
		{
			UnityEngine.UI.Selectable next = null;
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				if (Input.GetKeyDown(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftShift))
				{
					next = EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Selectable>().FindSelectableOnDown();
				}
				if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
				{
					next = EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Selectable>().FindSelectableOnUp();
				}
			}
			if (next != null)
			{
				UnityEngine.UI.Selectable selectable = next.GetComponent<UnityEngine.UI.Selectable>();
				if (selectable != null)
				{
					selectable.Select();
				}
			}
		}
	}
}