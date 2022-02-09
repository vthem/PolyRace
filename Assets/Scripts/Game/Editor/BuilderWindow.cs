using UnityEditor;

using UnityEngine;

namespace Game
{
	public class BuilderWindow : EditorWindow
	{
		private const string _builderAsset = "builder";

		[MenuItem("PolyRace/Builder")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(BuilderWindow));
		}

		private void OnGUI()
		{
			BuilderConfig builder = Resources.Load(_builderAsset) as BuilderConfig;
			if (GUILayout.Button("Build " + builder.VersionString))
			{
				Game.Config.GetAsset().SetDebug(false);
				Build(builder);
			}

			if (GUILayout.Button("Build (Debug) " + builder.VersionString))
			{
				Game.Config.GetAsset().SetDebug(true);
				Build(builder);
			}
		}

		private void IncrementBuild(BuilderConfig builder)
		{
			SerializedObject serializedObject = new UnityEditor.SerializedObject(builder);
			SerializedProperty buildCounterValue = serializedObject.FindProperty("_build");
			buildCounterValue.intValue++;
			serializedObject.ApplyModifiedProperties();
			AssetDatabase.SaveAssets();
		}

		private void Build(BuilderConfig builder)
		{
			if (!ValidateBuild())
			{
				return;
			}

			IncrementBuild(builder);

			Game.Config.GetAsset().SetDemo(false);
			for (int i = 0; i < builder._targets.Length; ++i)
			{
				Build(builder, i);
			}

			Game.Config.GetAsset().SetDemo(true);
			for (int i = 0; i < builder._targets.Length; ++i)
			{
				Build(builder, i);
			}
		}

		private bool ValidateBuild()
		{
			return true;
		}

		private void Build(BuilderConfig builder, int targetIndex)
		{
			bool isDemo = Game.Config.GetAsset().IsDemo;
			Debug.Log("BUILDING -- " + builder._targets[targetIndex] + " demo:" + isDemo);
			string versionString = builder.VersionString;
			if (Game.Config.GetAsset().IsDebug)
			{
				versionString += "d";
			}
			string root = "Builds/" + versionString + "/" + builder._targetsRoot[targetIndex];
			if (isDemo)
			{
				root = "Builds/" + versionString + "/Demo/" + builder._targetsRoot[targetIndex];
			}
			string binaryPath = root + "/" + builder._targetsBinary[targetIndex];
			UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(
				builder._levels,
				binaryPath,
				builder._targets[targetIndex],
				BuildOptions.None);

			UnityEngine.Debug.LogError("build complete ?!");
			//if (string.IsNullOrEmpty(report.)) {
			//    string appidFolder = "steam-appid/full/";
			//    if (isDemo) {
			//        appidFolder = "steam-appid/demo/";
			//    }
			//    FileUtil.CopyFileOrDirectory("Assets/Challenges", root + "/" + builder._targetsDataPath[targetIndex] + "/Challenges");
			//    if (builder._targets[targetIndex] == BuildTarget.StandaloneWindows) {
			//        System.IO.File.Copy(appidFolder + "steam_appid.txt", root + "/steam_appid.txt");
			//    } else if (builder._targets[targetIndex] == BuildTarget.StandaloneOSXIntel) {
			//        System.IO.File.Copy(appidFolder + "steam_appid.txt", root + "/" + builder._targetsDataPath[targetIndex] + "/MacOS/steam_appid.txt");
			//    } else if (builder._targets[targetIndex] == BuildTarget.StandaloneLinuxUniversal) {
			//        System.IO.File.Copy(appidFolder + "steam_appid.txt", root + "/steam_appid.txt");
			//    } else {
			//        Debug.LogWarning("unsupported build target:" + builder._targets[targetIndex]);
			//    }
			//}
		}
	}
}
