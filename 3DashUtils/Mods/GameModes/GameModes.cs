using _3DashUtils.ModuleSystem;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace _3DashUtils.Mods.GameModes;

public class GameModes : ButtonsModule
{
    internal enum GameMode
    {
        Cube,
        Ship,
        Wave,
        Hedron,
        UFO
    }
    public override string CategoryName => "GameModes";

    protected override List<KeyBindButton> Buttons { get; } = new();
    public GameModes()
    {
        Buttons.AddRange(new KeyBindButton[] {
            new("Flip Gravity", "Flips the players gravity.",
                new("FlipGravityShortcut",
                ()=>GravityFlip(),
                "Flips the players gravity.")
            ),
            new("Toggle Mini", "Toggles if the player is mini.",
                new("ToggleMiniShortcut",
                ()=>ToggleMini(),
                "Toggles if the player is mini.")
            ),
            new("Cube", "Changes the player into the cube gamemode.",
                new("CubeGamemodeButton",
                ()=>SetGamemode(GameMode.Cube),
                "Keybind for switching into cube gamemode.")
            ),
            new("Ship", "Changes the player into the ship gamemode.",
                new("ShipGamemodeButton",
                ()=>SetGamemode(GameMode.Ship),
                "Changes the player into the ship gamemode.")
            ),
            new("Wave", "Changes the player into the wave gamemode.",
                new("WaveGamemodeButton",
                ()=>SetGamemode(GameMode.Wave),
                "Changes the player into the wave gamemode.")
            ),
            new("Hedron (Ball)", "Changes the player into the hedron (a.k.a. ball) gamemode.",
                new("HedronGamemodeButton",
                ()=>SetGamemode(GameMode.Hedron),
                "Changes the player into the hedron (a.k.a. ball) gamemode.")
            ),
            new("UFO", "Changes the player into the UFO gamemode.",
                new("UFOGamemodeButton",
                ()=>SetGamemode(GameMode.UFO),
                "Changes the player into the UFO gamemode.")
            ),
        });
    }

    public override void OnGUI()
    {
        base.OnGUI();
        DrawAllKeybindButtons();
    }

    internal void SetGamemode(GameMode target)
    {
        Object.FindObjectOfType<PlayerScript>()?.SetCubeShape((int)target);
    }

    internal void ToggleMini()
    {
        var player = Object.FindObjectOfType<PlayerScript>();
        var half = new Vector3(0.5f, 0.5f, 0.5f);
        if (player.transform.localScale == half)
        {
            player.transform.localScale = Vector3.one;
        }
        else
        {
            player.transform.localScale = half;
        }
    }

    internal void GravityFlip(bool? down = null)
    {
        var player = Object.FindObjectOfType<PlayerScript>();
        float gravSign;
        if (down is bool b)
        {
            gravSign = (bool)down ? -1 : 1;
        }
        else
        {
            gravSign = Mathf.Sign(player.grav) * -1;
        }

        if (player.GetCubeShape() == (int)GameMode.Hedron)
        {
            if (Mathf.Sign(gravSign) > 0f)
            {
                if (player.gravDirection != 2)
                {
                    player.gravDirection = 2;
                    player.vsp *= 0.5f;
                }
            }
            else if (player.gravDirection != 0)
            {
                player.gravDirection = 0;
                player.vsp *= 0.5f;
            }
        }
        else if (Mathf.Sign(player.grav) != Mathf.Sign(gravSign))
        {
            player.grav *= -1f;
            player.vsp *= 0.5f;
        }
    }
}
