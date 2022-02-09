using System.Collections.Generic;
using System.IO;

namespace LevelGen
{
	using UnityEditor;

	using UnityEngine;

	internal class ColorMeshEditor : EditorWindow
	{
		private ColorProfile _profile;
		private float _scale;

		[MenuItem("PolyRace/ColorizeMesh")]
		public static void ShowWindow()
		{
			EditorWindow window = EditorWindow.GetWindow(typeof(ColorMeshEditor));
			window.titleContent.text = "ColorizeMesh";
		}

		private void OnGUI()
		{
			_profile = EditorGUILayout.ObjectField(_profile, typeof(ColorProfile), false) as ColorProfile;
			_scale = EditorGUILayout.FloatField("Scale", _scale);
			if (GUILayout.Button("Extract Mesh and Colorize") && Selection.activeObject != null && Selection.activeObject is Mesh)
			{
				Mesh mesh = (Mesh)Selection.activeObject;
				string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
				Mesh newMesh = new Mesh();
				CopyMesh(mesh, newMesh);
				CheckMesh(newMesh);
				ColorizeMesh(newMesh);
				AssetDatabase.CreateAsset(newMesh, path + "/" + mesh.name + "_" + _profile.name + ".asset");
			}
			if (GUILayout.Button("Colorize Mesh") && Selection.activeObject != null && Selection.activeObject is Mesh)
			{
				foreach (Object mesh in Selection.objects)
				{
					ColorizeMesh(mesh as Mesh);
				}
				AssetDatabase.SaveAssets();
			}
			if (Selection.activeObject != null)
			{
				EditorGUILayout.LabelField("type:" + Selection.activeObject.GetType().Name);
				EditorGUILayout.LabelField("path:" + AssetDatabase.GetAssetPath(Selection.activeObject));
			}
		}

		private static void CopyMesh(Mesh source, Mesh destination)
		{
			destination.triangles = new int[0];
			destination.vertices = source.vertices;
			destination.subMeshCount = source.subMeshCount;
			for (int i = 0; i < source.subMeshCount; ++i)
			{
				destination.SetTriangles(source.GetTriangles(i), i);
			}
			destination.colors = source.colors;
			destination.colors32 = source.colors32;
			destination.uv = source.uv;
			destination.normals = source.normals;
		}

		private void CheckMesh(Mesh mesh)
		{
			if (mesh.GetTopology(0) == MeshTopology.Triangles)
			{
				if (mesh.vertexCount != mesh.triangles.Length)
				{
					Vector3[] newVertices = new Vector3[mesh.triangles.Length];
					Vector3[] newNormals = new Vector3[mesh.triangles.Length];
					List<int>[] newTriangles = new List<int>[mesh.subMeshCount];
					Vector3[] vertices = mesh.vertices;
					int offset = 0;
					for (int sub = 0; sub < mesh.subMeshCount; ++sub)
					{
						int[] triangles = mesh.GetTriangles(sub);
						newTriangles[sub] = new List<int>();
						for (int i = 0; i < triangles.Length; i += 3)
						{
							int a = i;
							int b = i + 1;
							int c = i + 2;
							Vector3 pa = newVertices[a + offset] = vertices[triangles[a]];
							newTriangles[sub].Add(a + offset);

							Vector3 pb = newVertices[b + offset] = vertices[triangles[b]];
							newTriangles[sub].Add(b + offset);

							Vector3 pc = newVertices[c + offset] = vertices[triangles[c]];
							newTriangles[sub].Add(c + offset);

							newNormals[a + offset] = Vector3.Cross(pb - pa, pc - pa).normalized;
							newNormals[b + offset] = Vector3.Cross(pc - pb, pa - pb).normalized;
							newNormals[c + offset] = Vector3.Cross(pa - pc, pb - pc).normalized;
						}
						offset = newTriangles[sub].Count;
					}
					mesh.vertices = newVertices;
					mesh.normals = newNormals;
					//					mesh.normals = new Vector3[newVertices.Length];

					for (int sub = 0; sub < newTriangles.Length; ++sub)
					{
						mesh.SetTriangles(newTriangles[sub].ToArray(), sub);
					}
					mesh.uv = new Vector2[mesh.vertexCount];
				}
			}
		}

		private void ColorizeMesh(Mesh mesh)
		{
			Color[] colors = new Color[mesh.vertices.Length];
			Vector3[] vertices = mesh.vertices;
			// work on triangles now
			for (int sub = 0; sub < mesh.subMeshCount; ++sub)
			{
				int[] triangles = mesh.GetTriangles(sub);
				for (int i = 0; i < triangles.Length; i += 3)
				{
					Color color = _profile.BlendColor(vertices[triangles[i]] * _scale);
					colors[triangles[i]] = color;
					colors[triangles[i + 1]] = color;
					colors[triangles[i + 2]] = color;
				}
			}
			mesh.colors = colors;
		}
	}
}
