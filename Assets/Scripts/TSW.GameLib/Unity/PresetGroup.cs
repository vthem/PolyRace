#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Presets;

using UnityEngine;

public class PresetGroup : ScriptableObject
{
	public Preset preset = null;
	public Object[] objects = new Object[0];
}

[CustomEditor(typeof(PresetGroup))]
public class PresetGroupEditor : Editor
{
	private readonly SerializedProperty lookAtPoint;

	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Apply to group"))
		{
			PresetGroup pg = target as PresetGroup;
			int count = 0;
			foreach (Object obj in pg.objects)
			{
				GameObject gobj = obj as GameObject;
				if (!gobj)
				{
					Debug.Log($"skipping {obj.name} because not a GameObject");
					continue;
				}
				Component tapply = gobj.GetComponent(pg.preset.GetTargetFullTypeName());
				if (!tapply)
				{
					Debug.Log($"skipping {obj.name} because no {pg.preset.GetTargetFullTypeName()} on it");
					continue;
				}
				if (pg.preset.CanBeAppliedTo(tapply) && pg.preset.ApplyTo(tapply))
				{
					count++;
				}
				else
				{
					Debug.Log($"cannot apply {pg.preset.GetTargetFullTypeName()} to {obj.name}");
				}
			}
			if (count > 0)
			{
				AssetDatabase.SaveAssets();
				Debug.Log($"{count} assed modified");
			}
		}
		DrawDefaultInspector();
	}
}
#endif