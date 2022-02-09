using UnityEngine;

namespace TSW
{
	namespace Design
	{
		public class BasedOnPrefabAttribute : System.Attribute
		{
			private readonly string _prefabPath;
			public BasedOnPrefabAttribute(string prefabPath)
			{
				_prefabPath = prefabPath;
			}

			public string PrefabPath => _prefabPath;
		}

		public abstract class USingleton<T> : MonoBehaviour where T : Component, new()
		{
			private static T _instance;
			private bool _createSingletonCalled = false;
			private static bool _shutCalled = false;

			public static T Instance
			{
				get
				{
					if (_shutCalled)
					{
						return null;
					}

					if (_instance == null)
					{
						T comp = GameObject.FindObjectOfType<T>();
						if (comp != null)
						{
							_instance = comp;
						}
						else
						{
							GameObject obj = null;
							System.Attribute[] attrs = System.Attribute.GetCustomAttributes(typeof(T));
							foreach (System.Attribute attr in attrs)
							{
								if (attr is BasedOnPrefabAttribute)
								{
									BasedOnPrefabAttribute basedOnPrefabAttr = (BasedOnPrefabAttribute)attr;
									UnityEngine.Object original = Resources.Load(basedOnPrefabAttr.PrefabPath);
									if (original != null)
									{
										obj = GameObject.Instantiate(original) as GameObject;
										obj.name = "__auto_USingleton_BasedOn_" + original.name;
										break;
									}
								}
							}
							if (obj == null)
							{
								obj = new GameObject();
								_instance = obj.AddComponent<T>();
								obj.name = "__auto_USingleton_" + typeof(T).ToString();
							}
							else
							{
								_instance = obj.GetComponent<T>();
							}
						}

						USingleton<T> typedInstance = _instance as USingleton<T>;
						if (typedInstance != null)
						{
							typedInstance.CallSingletonCreate();
						}
						else
						{
							Debug.Log("could not instantiate " + typeof(T).Name);
						}
					}
					return _instance;
				}
			}

			private void Awake()
			{
				T obj = this as T;
				if (_instance == null)
				{
					_instance = obj;
					gameObject.name += "." + UnityEngine.Random.Range(0, 100000).ToString();
					CallSingletonCreate();
				}
				else if (_instance != obj)
				{
					DestroyImmediate(gameObject);
				}
			}

			private void CallSingletonCreate()
			{
				if (!_createSingletonCalled)
				{
					SingletonCreate();
					_createSingletonCalled = true;
				}
			}

			protected virtual void SingletonCreate()
			{
			}

			protected void Shut()
			{
				Debug.Log($"Shut {gameObject.name}");
				_shutCalled = true;
				_instance = null;
			}
		}
	}
}
