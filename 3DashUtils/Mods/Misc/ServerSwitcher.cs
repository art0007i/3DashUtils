using _3DashUtils.Mods.Player;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using HarmonyLib;
using System;
using System.Numerics;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerSwitcher : ToggleModule
{
    public override string CategoryName => "Misc";

    public override string ModuleName => "Server Switcher";

    public override float Priority => -1;


    public static ConfigOptionBase<string> serverAddress;
    public static string ServerAddress => serverAddress.Value;

    public override string Description => "Allows you to switch the games internal server.";

    protected override bool Default => false;

    private bool validateURL(string url)
    {
        bool valid = true;
        //if (!url.StartsWith("http://") || !url.StartsWith("https://")) valid = false;

        return valid;
    }

    public ServerSwitcher()
    {
        serverAddress = new TextInputConfig<string>(this, "URL", "http://delugedrip.com:30924", "The URL of the server.", validateURL);
    }

    public override void Start()
    {
        base.Start();
        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void SceneChanged(Scene arg0, Scene arg1)
    {
        if (Enabled)
        {
            ChangeURL(ServerAddress);
        }
        else
        {
            ChangeURL("http://delugedrip.com:30924");
        }
    }

    public override void OnToggle()
    {
        if(Enabled)
        {
            ChangeURL(ServerAddress);
        } else
        {
            ChangeURL("http://delugedrip.com:30924");
        }
    }

    private void ChangeURL(string url)
    {
        OnlineLevelsHub onlineLevelsHub = UnityEngine.Object.FindObjectOfType<OnlineLevelsHub>();
        if (onlineLevelsHub != null)
        {
            string get_url = $"{url}/3Dash/download";
            string get_recent_url = $"{url}/3Dash/recent";

            var urlField = typeof(OnlineLevelsHub).GetField("get_url", BindingFlags.NonPublic | BindingFlags.Instance);
            var recentUrlField = typeof(OnlineLevelsHub).GetField("get_recent_url", BindingFlags.NonPublic | BindingFlags.Instance);

            if (urlField != null && recentUrlField != null)
            {
                // Set the values of the private fields
                urlField.SetValue(onlineLevelsHub, get_url);
                recentUrlField.SetValue(onlineLevelsHub, get_recent_url);

                // Verify the values were set
                Debug.Log($"get_url set to: {urlField.GetValue(onlineLevelsHub)}");
                Debug.Log($"get_recent_url set to: {recentUrlField.GetValue(onlineLevelsHub)}");
            }

            // Invoke the private method LoadRecentLevels()
            var loadRecentLevelsMethod = typeof(OnlineLevelsHub).GetMethod("LoadRecentLevels", BindingFlags.NonPublic | BindingFlags.Instance);
            loadRecentLevelsMethod.Invoke(onlineLevelsHub, null);
        } else
        {
            //Debug.LogError("OnlineLevelsHub instance not found.");
        }

        Submission submission = UnityEngine.Object.FindObjectOfType<Submission>();
        if (submission != null)
        {
            string set_url = $"{url}/3Dash/upload";
            var setUrlField = typeof(Submission).GetField("set_url", BindingFlags.NonPublic | BindingFlags.Instance);
            if (setUrlField == null)
            {
                Debug.LogError("Field 'set_url' not found in Submission.");
                return;
            }

            // Set the values of the private fields
            setUrlField.SetValue(submission, set_url);

            // Verify the values were set
            Debug.Log($"set_url set to: {setUrlField.GetValue(submission)}");
        } else
        {
            //Debug.LogError("Submission instance not found.");
        }
    }

    public override void Update()
    {
        if (Enabled && serverAddress.Value != ServerAddress)
        {
            serverAddress.Value = ServerAddress;
            ChangeURL(ServerAddress);
        }
    }
}
