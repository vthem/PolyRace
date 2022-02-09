using UnityEngine;

using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TSW
{

	public static class UnityExtension
	{
		public static T GetComponent<T>(string objectPath)
			where T : Component
		{
			GameObject obj = GameObject.Find(objectPath);
			if (obj == null)
			{
				throw new System.Exception("Could not find object '" + objectPath + "'");
			}
			T comp = obj.GetComponent<T>();
			if (comp == null)
			{
				throw new System.Exception("Could not find component '" + typeof(T) + "' on object '" + objectPath + "'");
			}
			return comp;
		}

		/// <summary>
		/// Finds the component in object child. Throw an exception if the component does not exist
		/// </summary>
		/// <returns>The component in object.</returns>
		/// <param name="transform">Parent object.</param>
		/// <param name="objectName">Child object name.</param>
		/// <typeparam name="T">The type of the component.</typeparam>
		public static T FindComponentInObjectChild<T>(this Transform transform, string objectName)
			where T : Component
		{
			Transform child = transform.Find(objectName);
			if (child != null)
			{
				return child.GetComponent<T>();
			}
			else
			{
				throw new System.Exception("Object '" + objectName + "' not child of '" + transform.gameObject.name + "'");
			}
		}

		/// <summary>
		/// Finds the component in a target object.
		/// </summary>
		/// <returns>The component of the object</returns>
		/// <param name="objectName">Name of the target object.</param>
		/// <typeparam name="T">The type of the component.</typeparam>
		public static T FindComponentInObject<T>(string objectName)
			where T : Component
		{
			GameObject obj = GameObject.Find(objectName);
			if (obj == null)
			{
				throw new System.Exception("Object '" + objectName + "' could not be found");
			}
			T comp = obj.GetComponent<T>();
			if (comp == null)
			{
				throw new System.Exception("Object '" + objectName + "' does not contain component '" + typeof(T) + "'");
			}
			return comp;
		}

		/// <summary>
		/// Return the full path of a transform
		/// </summary>
		/// <returns>The full path.</returns>
		/// <param name="current">Current.</param>
		public static string GetFullPath(this Transform current)
		{
			if (current.parent == null)
			{
				return "/" + current.name;
			}
			return current.parent.GetFullPath() + "/" + current.name;
		}

		public static string GetFullPath(this Component component)
		{
			return component.transform.GetFullPath() + "/" + component.GetType().ToString();
		}

		public static void DestroyAllChild(this GameObject gameObject)
		{
			List<Transform> toDelete = new List<Transform>();
			foreach (Transform child in gameObject.transform)
			{
				toDelete.Add(child);
			}
			foreach (Transform del in toDelete)
			{
				del.gameObject.Destroy();
			}
		}

		public static void DestroyAllChild(this Transform transform)
		{
			transform.gameObject.DestroyAllChild();
		}

		public static T CreateGameObject<T>() where T : Component
		{
			GameObject obj = new GameObject(typeof(T).Name);
			return obj.AddComponent<T>();
		}

		public static void Destroy(this UnityEngine.Object obj)
		{
#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlaying)
			{
				GameObject.Destroy(obj);

			}
			else
			{
				GameObject.DestroyImmediate(obj);
			}
#else
			GameObject.Destroy(obj);
#endif
		}

#if UNITY_EDITOR
		public static T DrawComponentInspector<T>(string label, T component) where T : Component
		{
			component = EditorGUILayout.ObjectField(label, component, typeof(T), true) as T;
			if (null == component)
			{
				return component;
			}
			EditorGUI.indentLevel++;
			SerializedObject sobj = new SerializedObject(component);
			SerializedProperty prop = sobj.GetIterator();
			while (prop.NextVisible(true))
			{
				EditorGUILayout.PropertyField(prop);
			}
			sobj.ApplyModifiedProperties();
			prop.Reset();
			EditorGUI.indentLevel--;
			return component;
		}
#endif

		public static void WriteToFile(this Texture2D texture, string filename)
		{
#if !UNITY_WEBPLAYER
			System.IO.File.WriteAllBytes(filename, texture.EncodeToPNG());
#endif
		}

		public static Color32 Offset(this Color32 color, int offset)
		{
			color.r = ClampByte(color.r, offset);
			color.g = ClampByte(color.g, offset);
			color.b = ClampByte(color.b, offset);
			return color;
		}

		public static Color Offset(this Color color, float offset)
		{
			color.r = Mathf.Clamp01(offset + color.r);
			color.g = Mathf.Clamp01(offset + color.g);
			color.b = Mathf.Clamp01(offset + color.b);
			return color;
		}

		private static byte ClampByte(int b, int o)
		{
			if (b + o >= 255)
			{
				return 255;
			}
			if (b + o < 0)
			{
				return 0;
			}
			return (byte)(b + o);
		}


	}
}