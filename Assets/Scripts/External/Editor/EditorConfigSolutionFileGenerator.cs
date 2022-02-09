#if UNITY_EDITOR
using UnityEditor;

// based on https://github.com/zaikman/unity-editorconfig

public class ProjectFilePostprocessor : AssetPostprocessor
{
	public static string kEditorConfigProjectFindStr = "EndProject\r\nGlobal";
	public static string kEditorConfigProjectReplaceStr =
		"EndProject\r\n" +
		"Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Solution Items\", \"Solution Items\", \"{B24FE069-BB5F-4F16-BCDA-61C28EABC46B}\"\r\n" +
		"	ProjectSection(SolutionItems) = preProject\r\n" +
		"		.editorconfig = .editorconfig\r\n" +
		"	EndProjectSection\r\n" +
		"EndProject\r\n" +
		"Global";

	public static string kGlobalSectionFindStr = "EndGlobalSection\r\nEndGlobal";
	public static string kGlobalSectionReplaceStr =
		"EndGlobalSection\r\n" +
		"	GlobalSection(ExtensibilityGlobals) = postSolution\r\n" +
		"		SolutionGuid = {FD87994B-C032-4821-BD72-E057C33083EF}\r\n" +
		"	EndGlobalSection\r\n" +
		"EndGlobal";

	public static string OnGeneratedSlnSolution(string path, string content)
	{
		if (content.Contains(".editorconfig"))
			return content;
		content = content.Replace(kEditorConfigProjectFindStr, kEditorConfigProjectReplaceStr);
		content = content.Replace(kGlobalSectionFindStr, kGlobalSectionReplaceStr);
		return content;
	}

	public static string OnGeneratedCSProject(string path, string content)
	{
		return content;
	}
}
#endif