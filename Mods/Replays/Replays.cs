using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3DashUtils.Mods.Player;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using UnityEngine;

namespace _3DashUtils.Mods.Replays;

public class Replays : ToggleModule
{
    public override string CategoryName => "Replays";
    public static bool record => recordConfig.Value;
    private static ConfigOptionBase<bool> recordConfig;

    public static bool replay => replayConfig.Value;

    protected override bool Default => false;
    public override string Description => "gargar";

    private static ConfigOptionBase<bool> replayConfig;

    public double lastMS;
    public double currentMS = 0;

    public Dictionary<double, bool> testReplay;
    private bool lastClick = false;

    public override void OnGUI()
    {
        base.OnGUI();
        if(recordConfig.Value == true) {
            replayConfig.Value = false;
        } else if (replayConfig.Value == true) {
            recordConfig.Value = false;
        }
    }

    public Replays() {
        recordConfig = new ToggleConfigOption(this, "Record", false, "Records inputs.");
        replayConfig = new ToggleConfigOption(this, "Replays", false, "Replays inputs.");
    }

    public override void Update()
    {
        // here basically i need to check if the level started
        var path = Path.Combine(Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "Replays"), "testMacro.3DR"); //3dash replay (.3dr)
        if (recordConfig.Value)
        {
            lastMS = currentMS;

            if (lastMS > currentMS)
            {
                currentMS = lastMS + Time.timeSinceLevelLoad;
            }
            else
            {
                currentMS = Time.timeSinceLevelLoad;
            }

            if(lastClick && !GameObject.FindObjectOfType<PlayerScript>().jumpInputPressed)
            {
                testReplay.Add(currentMS, GameObject.FindObjectOfType<PlayerScript>().jumpInputPressed);
            }

            if(!lastClick && GameObject.FindObjectOfType<PlayerScript>().jumpInputPressed)
            {
                testReplay.Add(currentMS, GameObject.FindObjectOfType<PlayerScript>().jumpInputPressed);
            }

            lastClick = GameObject.FindObjectOfType<PlayerScript>().jumpInputPressed;
            File.WriteAllText(path, testReplay.ToString());
        }
    }

    /*
        this is how it should look:
        ---------------------------
        | textbox  (filename)     |
        ---------------------------
        
        -------------  ------------
        |  record   |  |   replay |
        -------------  ------------


        we need to have a macros folder in the mod folder
        where the macros are stored as files:
        
          click at 123ms    release at 256ms
                \/           \/
        {123ms: true, 256ms: false}

        single 1ms duration click
        {123ms: true, 124ms: false}

        click at 123ms
         \/
        123:1|256:0|

        123:1|124:0|
    */
}
