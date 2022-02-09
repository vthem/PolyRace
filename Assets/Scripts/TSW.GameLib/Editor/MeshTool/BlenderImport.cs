using UnityEditor;

using UnityEngine;

namespace TSW.MeshTool
{

	public class BlenderImport : AssetPostprocessor
	{
		public int _testTest;

		private void OnPreprocessModel()
		{
			if (assetPath.Contains(".blend"))
			{
				ModelImporter modelImporter = assetImporter as ModelImporter;
				if (modelImporter != null)
				{
					modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
				}
				else
				{
					Debug.Log("modelImporter is null!");
				}
			}
		}

		private void OnPostprocessModel(GameObject obj)
		{
			if (assetPath.Contains(".blend"))
			{
				MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
				if (meshFilter == null)
				{
					foreach (Transform t in obj.transform)
					{
						meshFilter = t.GetComponent<MeshFilter>();
						if (t != null)
						{
							BlenderRotation(meshFilter.sharedMesh);
							t.position = new Vector3(-t.position.x, t.position.y, -t.position.z);
							t.rotation = Quaternion.identity;
						}
					}
				}
				else
				{
					BlenderRotation(meshFilter.sharedMesh);
					obj.transform.position = new Vector3(-obj.transform.position.x, obj.transform.position.y, -obj.transform.position.z);
					obj.transform.rotation = Quaternion.identity;
				}

				//				string cmeshAssetPath = assetPath.Replace(".blend", "") + "_ColoredMesh.asset";
				//				bool exist;
				//				ColoredMesh cmesh = OpenMeshAsset<ColoredMesh>(obj, cmeshAssetPath, CreateColoredMeshObject, out exist);
				//				if (cmesh != null) {
				//					cmesh.UpdateMesh(obj);
				//				}
			}
		}

		private static void BlenderRotation(Mesh mesh)
		{
			Vector3[] vertices = new Vector3[mesh.vertices.Length];
			Vector3[] normals = new Vector3[mesh.vertices.Length];
			for (int i = 0; i < vertices.Length; ++i)
			{
				vertices[i] = new Vector3(-mesh.vertices[i].x, mesh.vertices[i].z, mesh.vertices[i].y);
				normals[i] = new Vector3(-mesh.normals[i].x, mesh.normals[i].z, mesh.normals[i].y);
			}
			mesh.vertices = vertices;
			mesh.normals = normals;

			//for each submesh, we invert the order of vertices for all triangles
			//for some reason changing the vertex positions flips all the normals???
			//			for (int submesh = 0; submesh < mesh.subMeshCount; submesh++) {
			//				int[] triangles = mesh.GetTriangles(submesh);
			//				for (int i = 0; i < triangles.Length; i += 3) {
			//					int tmp = triangles[i];
			//					triangles[i] = triangles[i + 2];
			//					triangles[i + 2] = tmp;
			//				}
			//				mesh.SetTriangles(triangles, submesh);
			//			}

			//recalculate other relevant mesh data
			//			mesh.RecalculateNormals();
			//			mesh.RecalculateBounds();
		}

		private delegate UnityEngine.Object CreateAssetDelegate();

		private static UnityEngine.Object CreateMeshObject()
		{
			return new Mesh();
		}

		private static T OpenMeshAsset<T>(GameObject selection, string assetPath, CreateAssetDelegate createAssetDelegate, out bool exist) where T : UnityEngine.Object
		{
			T existing = AssetDatabase.LoadMainAssetAtPath(assetPath) as T;
			exist = true;
			if (existing == null)
			{
				existing = createAssetDelegate() as T;
				AssetDatabase.CreateAsset(existing, assetPath);
				exist = false;
				//				AssetDatabase.SaveAssets();
				//				AssetDatabase.Refresh();
			}
			return existing;
		}
	}
}