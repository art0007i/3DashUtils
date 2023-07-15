using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetBundleMaker : MonoBehaviour
{
    public Material mat;
    public bool CopyToUtilsProject = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void BuildBundles()
    {
#if UNITY_EDITOR
        AssetBundleBuild[] buildMap = new AssetBundleBuild[2];

        buildMap[0].assetBundleName = "utilsAssets";
        buildMap[1].assetBundleName = "utilsIconKit";

        var rootPath = Path.GetDirectoryName(Application.dataPath);
        var path = Path.Combine(Application.dataPath, "BundleMe");
        var assetEnum = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Where(
            (s) => {
                var ex = Path.GetExtension(s);
                // C# FILES CANNOT BE BUNDLED!! MUST DO IT MANUALLY
                return ex != ".meta" && ex != ".cs";
            });
        // map 0 has no unity scenes
        var map0 = assetEnum.Where((s) => !s.EndsWith(".unity"));
        buildMap[0].assetNames = map0.Select((s) =>  Path.GetRelativePath(rootPath, s)).ToArray();
        // map 1 has only unity scenes, because they don't bundle together
        var map1 = assetEnum.Where((s) => s.EndsWith(".unity"));
        buildMap[1].assetNames = map1.Select((s) =>  Path.GetRelativePath(rootPath, s)).ToArray();

        // rename the bundled assets for easier access
        buildMap[0].addressableNames = map0.Select((s) => "Utils/" + Path.GetRelativePath(path, s)).ToArray();
        buildMap[1].addressableNames = map1.Select((s) => "Utils/" + Path.GetRelativePath(path, s)).ToArray();

        for (var i = 0; i < buildMap.Length; ++i)
        {
            var m = buildMap[i];
            Debug.Log( "============== MAP " + i + "==============");
            for (int j = 0; j < m.assetNames.Length; j++)
            {
                Debug.Log($"{m.assetNames[j]} -> {m.addressableNames[j] ?? "NULL"}");
            }
        }
        //string[] myAssets = new string[2];
        //myAssets[0] = "Assets/UnlitVertex.shader";
        //myAssets[1] = "Assets/VColMat.mat";

        //buildMap[0].assetNames = myAssets;

        BuildPipeline.BuildAssetBundles("Assets/Bundles", buildMap, 0, BuildTarget.StandaloneWindows);
        if (!CopyToUtilsProject) return;
        var srcPath = Path.Combine(Application.dataPath, "Bundles");
        var dstPath = Path.Combine(Path.GetDirectoryName(rootPath), "3DashUtils", "Resources");
        Debug.Log("Building Complete... Copying to " + dstPath);
        foreach (var bundle in buildMap)
        {
            var src = Path.Combine(srcPath, bundle.assetBundleName);
            var dst = Path.Combine(dstPath, bundle.assetBundleName);
            File.Copy(src, dst);
        }
#endif
    }

    public void DebugMaterial()
    {

        var m = this.mat;
        var s = m.shader;
        var Log = new Action<string>(Debug.Log);
        for (int i = 0; i < s.GetPropertyCount(); i++)
        {
            var n = s.GetPropertyName(i);
            switch (s.GetPropertyType(i))
            {
                case UnityEngine.Rendering.ShaderPropertyType.Color:
                    Log($"{n} (col) -> {m.GetColor(n)}");
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Vector:
                    Log($"{n} (vec) -> {m.GetVector(n)}");
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Float:
                    Log($"{n} (flt) -> {m.GetFloat(n)}");
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Range:
                    Log($"{n} (rng) -> {m.GetFloat(n)}");
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Texture:
                    Log($"{n} (tex) -> {m.GetTexture(n)}");
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Int:
                    Log($"{n} (int) -> {m.GetInteger(n)}");
                    break;
                default:
                    break;
            }
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(AssetBundleMaker))]
public class AssetBundleMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var t = (AssetBundleMaker)target;
        base.OnInspectorGUI();
        if(GUILayout.Button("build bundle"))
        {
            t.BuildBundles();
        }
        if (GUILayout.Button("a"))
        {
            t.DebugMaterial();
        }
    }
}
#endif
