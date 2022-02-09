using UnityEditor;

using UnityEngine;

namespace TSW.Unity
{
	public static class UnityEditorExtension
	{
		public static T CreateScriptableObject<T>() where T : ScriptableObject
		{
			T obj = ScriptableObject.CreateInstance<T>();
			CreateAsset(obj);
			return obj;
		}

		public static void CreateAsset(Object obj)
		{
			string directory = "Assets/";
			if (Selection.activeInstanceID != 0)
			{
				directory = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeInstanceID));
			}
			string path = directory + "/" + obj.GetType().ToString() + ".asset";
			path = AssetDatabase.GenerateUniqueAssetPath(path);
			AssetDatabase.CreateAsset(obj, path);
			AssetDatabase.Refresh();
		}

		[MenuItem("TSW/Create ScriptableObject Instance")]
		public static void MenuCreateScriptableObjectInstance()
		{
			if (Selection.activeInstanceID != 0)
			{
				try
				{
					MonoScript ms = Selection.activeObject as MonoScript;
					if (ms != null)
					{
						string className = ms.GetClass().Name;
						Object obj = ScriptableObject.CreateInstance(className);
						CreateAsset(obj);
					}

				}
				catch (System.Exception)
				{
				}
			}
		}
	}
}
