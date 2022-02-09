using UnityEngine;

namespace TSW
{
	public static class PlayerPrefsObject
	{
		public static void Set(string key, object obj)
		{
			try
			{
				PlayerPrefs.SetString(key, ObjectSerializer.SerializeBase64(obj));
			}
			catch (System.Exception ex)
			{
				Debug.Log("Unable to save object to players prefs Type:" + obj.GetType().Name + " exception:" + ex.Message);
			}
		}

		public static T Get<T>(string key) where T : class
		{
			if (!PlayerPrefs.HasKey(key))
			{
				return null;
			}
			try
			{
				return ObjectSerializer.DeserializeBase64<T>(PlayerPrefs.GetString(key));
			}
			catch (System.Exception)
			{
				PlayerPrefs.DeleteKey(key);
				return null;
			}
		}
	}
}
