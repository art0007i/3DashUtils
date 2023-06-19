using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _3DashUtils.Mods.Editor;

public class MoreLevelSlots : ToggleModule
{
    public override string CategoryName => "Editor";

    public override string ModuleName => "More Level Slots";

    public override string Description => "Allows you to have infinite level slots so you can create as many levels as you want!";

    protected override bool Default => true;

    private void DupeFile(SaveSelect saveSelect, int n = 1)
    {
        if (n <= 0) return;
        var firstButton = saveSelect.fileTexts[0].transform.parent;
        var coll = saveSelect.fileTexts.AsEnumerable();
        for(int i = 0; i < n; i++)
        {
            var newButton = GameObject.Instantiate(firstButton);
            newButton.transform.SetParent(firstButton.parent, false);
            var btnIndex = firstButton.parent.childCount;
            newButton.name = $"Button [{btnIndex}]";
            // TODO: color the buttons nicer
            var btnComp = newButton.GetComponentInChildren<Button>();
            btnComp.onClick = new(); // reset it so that it doesn't trigger the original action
            btnComp.onClick.AddListener(() => saveSelect.FileButton(btnIndex.ToString()));
            coll = coll.Append(newButton.GetComponentInChildren<TextMeshProUGUI>());
        }
        saveSelect.fileTexts = coll.ToArray();
        saveSelect.UpdateButtons();
    }

    public override void Awake()
    {
        SceneManager.activeSceneChanged += (oldScene, newScene) =>
        {
            if (!Enabled || newScene.name != "Save Select") return;


            var buttonsRoot = newScene.GetRootGameObjects().Select((g) => g.transform.Find("Buttons")).FirstOrDefault((g) => g != null);
            if(buttonsRoot != null)
            {
                var saveSelect = GameObject.Find("SaveSelect").GetComponent<SaveSelect>();

                var savePath = Path.GetDirectoryName(SaveSelect.GetPath("0"));
                var largetFile = Directory.GetFiles(savePath, "save*.dat").Select(file =>
                    int.Parse(string.Concat(Path.GetFileName(file).Substring(4).TakeWhile(c => char.IsDigit(c))))
                ).Max();

                // 10 default files, dont need to dupe them
                // 1 extra so u can always have a free slot :)
                DupeFile(saveSelect, (largetFile - 10) + 1);
                
                var go = buttonsRoot.gameObject;
                // this is the actual size of those buttons. wtf
                // 684.4152, 126.1679
                Vector2 buttonSize = new(650, 125);
                Vector2 space = new Vector2(20, 30);
                float halfWidth = buttonSize.x + (space.x / 2);
                var grid = go.AddComponent<GridLayoutGroup>();
                var rect = go.GetComponent<RectTransform>();
                var fitter = go.AddComponent<ContentSizeFitter>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                grid.cellSize = buttonSize;
                grid.spacing = space;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                var newParent = new GameObject("ScrollParent");
                newParent.transform.SetParent(buttonsRoot.parent, false);
                var scroll = newParent.AddComponent<ScrollRect>();
                var scrollTransform = newParent.GetComponent<RectTransform>();
                scrollTransform.anchorMax = new Vector2(0.5f, 1);
                scrollTransform.anchorMin = new Vector2(0.5f, 0);
                scrollTransform.offsetMax = new Vector2(halfWidth, -200);
                scrollTransform.offsetMin = new Vector2(-halfWidth, 20);
                newParent.AddComponent<Image>();
                newParent.AddComponent<Mask>().showMaskGraphic = false;

                buttonsRoot.SetParent(scroll.transform, false);

                scroll.content = rect;
                scroll.horizontal = false;
                scroll.scrollSensitivity = 25;

                // scroll to top
                scroll.verticalNormalizedPosition = 1;
            }
        };
    }
}
