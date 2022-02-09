using TSW.Struct;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

public class TestExplosion : MonoBehaviour
{
    public PrefabGroup[] groups;

    public List<GameObject> obstacles = new List<GameObject>();

    public int nextIndex = 0;

    public float forceFactor = 15f;


    public void Instantiate()
    {
        var pos = new Vector3(0, 0, 0);
        for (int i = 0; i < groups.Length; ++i)
        {
            var objs = groups[i].Objects;
            foreach (var obj in objs)
            {
                var nobj = GameObject.Instantiate(obj, pos, Quaternion.identity);
                obstacles.Add(nobj);
                pos += new Vector3(5, 0, 0);
            }
        }
    }

    public void ExplodeNext()
    {
        nextIndex = nextIndex % obstacles.Count;
        obstacles[nextIndex].GetComponent<ObstacleExplosion>().Explode(Vector3.forward, Vector3.up, forceFactor, false);
        obstacles[nextIndex].SetActive(false);
        nextIndex++;
    }

    public IEnumerator ExplodeAll()
    {
        nextIndex = 0;
        while (nextIndex < obstacles.Count)
        {
            ExplodeNext();
            yield return new WaitForSeconds(0.1f);
        }
    }
}


[CustomEditor(typeof(TestExplosion))]
public class TestExplosionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Instantiate"))
            {
                (target as TestExplosion).Instantiate();
            }
            if (GUILayout.Button("Next"))
            {
                (target as TestExplosion).ExplodeNext();
            }
            if (GUILayout.Button("Explode All"))
            {
                (target as MonoBehaviour).StartCoroutine((target as TestExplosion).ExplodeAll());
            }
        }
    }
}
#endif