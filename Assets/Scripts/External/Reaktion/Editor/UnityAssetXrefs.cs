using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

/// <summary>
/// Right-click on an asset, choose "What objects use this?" or "What objects in scene uses this?"
/// also, "Select All Missing refs" will point out every asset which has "Missing" object references in it.
/// 
/// by Matt "Trip" Maker, Monstrous Company :: http://monstro.us
/// 
/// from http://unifycommunity.com/wiki/index.php?title=UnityAssetXrefs
/// 
/// </summary>
public class UnityAssetXrefs {

    public static bool ADependsOnB(Object obj, Object selectedObj) {
        if (selectedObj == null) return false;
        //optionally, exclude self.
        if (selectedObj == obj) return false;

        Object[] dependencies = EditorUtility.CollectDependencies(new Object[1] { obj });
        if (dependencies.Length < 2) return false; // if there's only one, it's us.
                                                   //Debug.Log(obj.name + " has " + dependencies.Length + " dependencies", obj);

        foreach (UnityEngine.Object dep in dependencies)
            if (dep == selectedObj)
                return true;
        return false;
    }

    /* This is still useful, because it finds places where you have dragged the item onto scene objects.
	   Unity's built-in feature "Find References in Scene." does not find those uses. - MM */
    [MenuItem("Assets/What objects in scene use this?", false, 20)]
    public static void SelectSceneUsesOfAsset() {
        Object cur = Selection.activeObject;
        //optionally tell the user what's going on
        Object[] sceneObjects = Object.FindObjectsOfType<Object>();
        Debug.Log("You have asked which of " + sceneObjects.Length + " assets make use of " + cur.name + ", a " + cur.GetType().ToString() + ".", cur);

        List<Object> results = CollectReverseDependenciesInCurrentScene(cur);
        //TODO get only main assets of above
        Debug.Log(results.Count() + " uses in scene found for this asset.", cur);
        //printAssetPaths(results);
        foreach (Object result in results)
            Debug.Log(result.name + " / " + PrefabUtility.GetCorrespondingObjectFromSource(result).GetType().ToString(), result);
        setSelection(results);
    }
    public static List<Object> CollectReverseDependenciesInCurrentScene(Object b) {
        Object[] sceneObjects = Object.FindObjectsOfType<Object>();
        return sceneObjects.Where(a => ADependsOnB(a, b)).ToList();
    }

    [MenuItem("Assets/What objects use this?", false, 20)]
    public static void SelectUsesOfAsset() {
        if (Selection.objects.Length != 1) {
            Debug.LogWarning("Please select just one asset");
            return;
        }
        Object cur = Selection.activeObject;
        float startTime = Time.realtimeSinceStartup;
        List<Object> results = CollectReverseDependencies(cur);
        Debug.Log(results.Count() + " reverse dependencies found for this asset in " + (Time.realtimeSinceStartup - startTime).ToString() + " secs.", cur);
        printAssetPaths(results);
        setSelection(results);
    }

    public static List<Object> CollectReverseDependencies(Object b) {
        return allAssets.Where(a => ADependsOnB(a, b)).ToList();
    }

    public static List<Object> CollectReverseDependencies(Object[] objs) {
        List<Object> ret = new List<Object>();
        foreach (Object obj in objs)
            ret.AddRange(CollectReverseDependencies(obj));
        return ret;
    }


    public static bool hasMissingRef(Object obj) {
        if (!obj) return false;
        Object[] dependencies = EditorUtility.CollectDependencies(new Object[1] { obj });
        foreach (Object dep in dependencies)
            if (dep == null)
                return true;
        return false;
    }

    [MenuItem("Assets/Select All Missing refs")]
    public static void SelectMissingRefs() {
        float startTime = Time.realtimeSinceStartup;
        List<Object> results = CollectMissingRefs();
        Debug.Log(results.Count() + " missing references found in" + (Time.realtimeSinceStartup - startTime).ToString() + " secs.");
        printAssetPaths(results);
        setSelection(results);
    }
    public static List<Object> CollectMissingRefs() {
        return allAssets.Where(x => hasMissingRef(x)).ToList();
    }

    public static void printAssetPaths(List<Object> objs) {
        foreach (Object obj in objs)
            Debug.Log(AssetDatabase.GetAssetPath(obj), obj);
    }

    public static void setSelection(List<Object> results) {
        if (results.Count() > 0)
            Selection.objects = results.ToArray();
    }


    // everything below this line would normally be in other libraries I've made, but I've brought these versions of them into here for simplicity
    public static List<Object> allAssets {
        get {
            // get every single one of the files in the Assets folder.
            List<FileInfo> files = DirSearch(new DirectoryInfo(Application.dataPath), "*.*");

            // now make them all into Asset references.
            List<Object> assetRefs = new List<Object>();

            foreach (FileInfo fi in files) {
                if (fi.Name.StartsWith(".")) continue; // Unity ignores dotfiles.
                assetRefs.Add(AssetDatabase.LoadMainAssetAtPath(getRelativeAssetPath(fi.FullName)));
            }
            return assetRefs;
        }
    }

    public static string fixSlashes(string s) {
        const string forwardSlash = "/";
        const string backSlash = "\\";
        return s.Replace(backSlash, forwardSlash);
    }

    /// given a path to a node in the filesystem, lop off anything above the project Assets folder in the pathname so it can work with UnityEditor's built-in commands
    public static string getRelativeAssetPath(string pathName) {
        //dataPath uses forward slashes on all platforms now
        return fixSlashes(pathName).Replace(Application.dataPath, "Assets");
    }

    // given a folder and a search filter, return a list of file references
    // (in the unlikely event you have some filesystem arrangement with recursive "hard links", be aware this may not work out well for you)
    public static List<FileInfo> DirSearch(DirectoryInfo d, string searchFor) {
        List<FileInfo> founditems = d.GetFiles(searchFor).ToList();
        // Add (by recursing) subdirectory items.
        DirectoryInfo[] dis = d.GetDirectories();
        foreach (DirectoryInfo di in dis)
            founditems.AddRange(DirSearch(di, searchFor));

        return (founditems);
    }

}