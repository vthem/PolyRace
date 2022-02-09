using UnityEditor;

using UnityEngine;

namespace TSW.Noise
{
	public class SourcePreviewWindow : EditorWindow
	{
		private int _samplingWidth = 48;
		private int _samplingheight = 192;
		private float _samplingStep = 10f;
		private float _previewScale = .4f;
		private Texture _previewTexture;
		private Source _source;
		private float _lastValue;
		private bool _autoRefresh;
		private bool _lockSource;
		private double _lastUpdate;

		// Add menu named "My Window" to the Window menu
		[MenuItem("TSW/Noise/Preview")]
		private static void Init()
		{
			// Get existing open window or if none, make a new one:
			SourcePreviewWindow window = (SourcePreviewWindow)EditorWindow.GetWindow(typeof(SourcePreviewWindow));
			window.Show();
		}

		[MenuItem("TSW/Noise/New Highest Modifier")]
		private static void NewHighestModifier()
		{
			TSW.Unity.UnityEditorExtension.CreateScriptableObject<Highest>();
		}

		private void OnGUI()
		{
			EditorGUILayout.BeginVertical();
			_samplingWidth = EditorGUILayout.IntField("Sampling Width", _samplingWidth);
			_samplingheight = EditorGUILayout.IntField("Sampling Height", _samplingheight);
			_samplingStep = EditorGUILayout.FloatField("Sampling Step", _samplingStep);
			_previewScale = EditorGUILayout.FloatField("Preview Scale", _previewScale);
			_autoRefresh = EditorGUILayout.Toggle("Auto Refresh", _autoRefresh);
			_lockSource = EditorGUILayout.Toggle("Lock Source", _lockSource);


			EditorGUILayout.EndVertical();
			if (_previewTexture != null)
			{
				EditorGUI.DrawPreviewTexture(new Rect(25, 120, Mathf.RoundToInt(_samplingWidth * _previewScale * _samplingStep), Mathf.RoundToInt(_samplingheight * _previewScale * _samplingStep)), _previewTexture, null, ScaleMode.StretchToFill);
			}
		}

		private void OnSelectionChange()
		{
			if (_lockSource)
			{
				return;
			}
			Source source = Selection.activeObject as Source;
			if (source != null && source.IsValid())
			{
				_source = source;
				_previewTexture = CreateTexture(_source);
				Repaint();
				UpdateLastValue();
			}
			else
			{
				_previewTexture = null;
			}
		}

		private void Update()
		{
			if (_autoRefresh && _source != null && _source.IsValid() && EditorApplication.timeSinceStartup > _lastUpdate + 0.5f)
			{
				_lastUpdate = EditorApplication.timeSinceStartup;
				_previewTexture = CreateTexture(_source);
				Repaint();
			}
		}

		private bool UpdateLastValue()
		{
			if (_source == null || !_source.IsValid())
			{
				return false;
			}
			float v = _source.GetFloat(new Vector3(10.5f, 20.5f, 30.1f));
			bool update = v != _lastValue;
			_lastValue = v;
			return update;
		}

		public Texture2D CreateTexture(Source source)
		{
			Texture2D texture = new Texture2D(_samplingWidth, _samplingheight)
			{
				filterMode = FilterMode.Point
			};
			UnityEngine.Color[] pixels = new Color[texture.width * texture.height];
			Vector3 co = Vector3.zero;
			float sample = 0f;
			for (int z = 0; z < texture.height; z++)
			{
				for (int x = 0; x < texture.width; x++)
				{
					co.x = x * _samplingStep;
					co.z = z * _samplingStep;
					sample = source.GetFloat(co);
					pixels[z * texture.width + x].a = 1f;
					pixels[z * texture.width + x].r = sample;
					pixels[z * texture.width + x].g = sample;
					pixels[z * texture.width + x].b = sample;
				}
			}
			pixels[0] = UnityEngine.Color.green;
			pixels[pixels.Length - 1] = UnityEngine.Color.red;
			texture.SetPixels(pixels);
			texture.Apply();
			return texture;
		}
	}
}