using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
	public class BuilderConfig : ScriptableObject
	{
		public string _version;

		[SerializeField]
		private int _build = 0;

		public static BuilderConfig Instance()
		{
			BuilderConfig builder = Resources.Load("builder") as BuilderConfig;
			if (builder == null)
			{
				throw new System.Exception("Could not find builder in Assets/Resources/builder");
			}
			return builder;
		}

		public string VersionString => _version + "." + _build;

#if UNITY_EDITOR
		public BuildTarget[] _targets = new BuildTarget[] {
			BuildTarget.StandaloneWindows
		};
#endif

		public string[] _targetsRoot = new string[] {
			"Windows",
			"OSX",
			"Linux"
		};

		public string[] _targetsBinary = new string[] {
			"PolyRace.exe",
			"PolyRace.app",
			"PolyRace"
		};

		public string[] _targetsDataPath = new string[] {
			"PolyRace_Data",
			"PolyRace.app/Contents",
			"PolyRace_Data"
		};

		public string[] _levels = new string[] {
			"Assets/Scenes/race.unity",
		};
	}
}


