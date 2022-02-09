using UnityEngine;
using UnityEngine.UI;

namespace TSW.Loca
{
	using LocalizationSystem = DirtyLoca;

	[RequireComponent(typeof(Text))]
	public class LocalizedText : MonoBehaviour
	{
		public string localizedKey = "localization key";
		private Text textObject;

		private void Start()
		{
			textObject = GetComponent<Text>();
			LocalizationSystem.OnLanguageChanged += OnChangeLanguage;

			//Run the method one first time
			OnChangeLanguage();
		}

		private void OnDestroy()
		{
			LocalizationSystem.OnLanguageChanged -= OnChangeLanguage;
		}

		private void OnChangeLanguage()
		{
			textObject.text = LocalizationSystem.GetTextValue(localizedKey);
		}
	}
}