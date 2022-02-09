using UnityEngine;

namespace Game
{
	public class Config : ScriptableObject
	{
		public enum BootType
		{
			Intro,
			Menu,
			Sandbox
		}

		public float MaxAcceleration = 160f;

		public Vector3 StartPosition = new Vector3(240, 10, 230);

		public float ChallengeTimerDuration = 600f;

		public float MaxGhostInterval = .1f;

		public bool AutoAcceleration;

		[SerializeField]
		private bool _isDemo;

		public bool IsDemo => _isDemo;

		public BootType GameBootType = BootType.Intro;

		public bool QuickSaveMission = false;

		[SerializeField]
		private bool _isDebug;

		public bool IsDebug => _isDebug;

		public static Config GetAsset()
		{
			return Resources.Load("Config") as Config;
		}

#if UNITY_EDITOR
		public void SetDemo(bool state)
		{
			SetProperty("_isDemo", state);
		}

		public void SetDebug(bool state)
		{
			SetProperty("_isDebug", state);
		}

		private void SetProperty(string name, bool state)
		{
			UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
			UnityEditor.SerializedProperty demo = serializedObject.FindProperty(name);
			demo.boolValue = state;
			serializedObject.ApplyModifiedProperties();
			UnityEditor.AssetDatabase.SaveAssets();
		}
#endif
	}
}