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
    private const float ERROR_DURATION = 2;

    private float lastErrorTime = ERROR_DURATION - 1;

    public override float Priority => 1;

    public override string CategoryName => "Editor";
    public override string ModuleName => "Level Import/Export";

    public override void OnGUI()
    {

        var text = "Import Level";
        if(Time.realtimeSinceStartup - lastErrorTime < ERROR_DURATION)
        {
            text = "<color=red>Error in import!</color>";
        }

        var importTip = this.GenerateTooltip("Override the current save with a level JSON from your clipboard.");
        if (GUILayout.Button(new GUIContent(text,importTip)))
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

        var exportTip = this.GenerateTooltip("Copy the current level JSON into your clipboard. Also works in user levels!");
        if (GUILayout.Button(new GUIContent("Export Level", exportTip)))
        {
            var scn = SceneManager.GetActiveScene();
            var editor = scn.GetRootGameObjects().Where((obj) => obj.GetComponent<LevelEditor>()).First().GetComponent<LevelEditor>();

            var levelObject = LevelEditor.ExportToLevelObject();
            var json = editor.LevelToJSON(levelObject);
            GUIUtility.systemCopyBuffer = json;
        }
    }
}

