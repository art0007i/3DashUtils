using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using _3DashUtils.ModuleSystem;
using UnityEngine.SceneManagement;

namespace _3DashUtils.Mods.Editor;
public class LevelImportExport : ModuleBase
{
    private float lastErrorTime = -3;
    public override string CategoryName => "Editor";

    public override void OnGUI()
    {
        var text = "Import Level";
        if(Time.realtimeSinceStartup - lastErrorTime < 2)
        {
            text = "<color=red>Error in import!</color>";
        }

        if(GUILayout.Button(text))
        {
            var scn = SceneManager.GetActiveScene();
            var editor = scn.GetRootGameObjects().Where((obj) => obj.GetComponent<LevelEditor>()).First().GetComponent<LevelEditor>();

            try
            {
                var json = GUIUtility.systemCopyBuffer;
                var levelObject = editor.JSONToLevel(json);
                LevelEditor.ImportFromLevelObject(levelObject);
            } catch
            {
                lastErrorTime = Time.realtimeSinceStartup;
            }
        }
        
        if(GUILayout.Button("Export Level"))
        {
            var scn = SceneManager.GetActiveScene();
            var editor = scn.GetRootGameObjects().Where((obj) => obj.GetComponent<LevelEditor>()).First().GetComponent<LevelEditor>();

            var levelObject = LevelEditor.ExportToLevelObject();
            var json = editor.LevelToJSON(levelObject);
            GUIUtility.systemCopyBuffer = json;
        }
    }
}

