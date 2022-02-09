using System.Collections.Generic;

using UnityEngine;

namespace Game.DebugUtils
{
	public class ChangeLanguage : MonoBehaviour
	{

		// Use this for initialization
		private void Start()
		{

		}

		// Update is called once per frame
		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.PageUp))
			{
				NextLanguage(1);
				Debug.Log("lang:" + TSW.Loca.DirtyLoca.CurrentLanguageCode);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.PageDown))
			{
				NextLanguage(-1);
				Debug.Log("lang:" + TSW.Loca.DirtyLoca.CurrentLanguageCode);
			}
		}

		private void NextLanguage(int offset)
		{
			List<string> langs = new List<string>(TSW.Loca.DirtyLoca.AvailableLanguageCode);
			string current = TSW.Loca.DirtyLoca.CurrentLanguageCode;
			int index = langs.IndexOf(current);
			index += offset;
			if (index >= langs.Count)
			{
				index = 0;
			}
			if (index < 0)
			{
				index = langs.Count - 1;
			}
			TSW.Loca.DirtyLoca.UseLanguage(langs[index]);
			Player.PlayerManager.Instance.SetLanguage(langs[index]);
		}
	}
}