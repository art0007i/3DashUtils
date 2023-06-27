# 3DashUtils

A utility mod for [3Dash](https://delugedrop.itch.io/3dash)

It adds a menu (toggled by pressing tab) which gives you access to a bunch of different utilities and fixes.

You can also hold left shift to edit keybinds.

We have a discord server! Check it out here: https://discord.gg/dSBMSe6CZC

## Installation

There are 2 versions of 3DashUtils available, one for [BepInEx](https://github.com/BepInEx/BepInEx) and one for [MelonLoader](https://melonwiki.xyz/)

### BepInEx
1. Download [BepInEx](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.21)
2. Extract the downloaded zip file on top of your game (the BepInEx folder should be in the same folder as 3Dash.exe)
3. Download [3DashUtils-BepInEx](https://github.com/art0007i/3DashUtils/releases/latest/download/3DashUtils-BepInEx.zip)
4. Extract the downloaded zip file into the `BepInEx` directory. If it doesn't exist you may need to create it yourself.
After everything is done your folder structure should have these files:
```
3Dash Windows v1.2.1
├── BepInEx/
│   └── plugins/
│       └── 3DashUtils/
│           ├── Resources
│           └── 3DashUtils.dll
├── doorstop_config.ini
├── 3Dash.exe
└── winhttp.dll
```
### MelonLoader
1. [Install MelonLoader](https://melonwiki.xyz/#/?id=automated-installation).
2. Download [3DashUtils-MelonLoader](https://github.com/art0007i/3DashUtils/releases/latest/download/3DashUtils-MelonLoader.zip)
3. Extract the downloaded zip file into the game directory.
After everything is done your folder structure should have these files:
```
3Dash Windows v1.2.1
├── Mods/
│   └── 3DashUtils.dll
├── UserData/
│   └── 3DashUtils/
│       └── Resources/
│           ├── jumpscare.mp3
│           └── jumpscare.png
└── 3Dash.exe
```

## Features

- General
  - Key Binding system
  - Feature rich UtilityMenu with multiple windows
  - Remember settings such as "show path" and "volume"
  - Apply volume setting to main menu music
- Editor
  - CameraBypass (Allows you to move the editor camera outside of the normal area)
  - MoreLevelSlots (Allows you to have infinite editor levels)
  - LevelImportExport (Allows you to export and import levels to JSON format)
  - ObjectLimitBypass (Removes the limit for placing objects)
- Player
  - Speedhack
  - NoClip
  - FastDeath (Removes/shortens the death animation)
  - NoPauseSuicide (Prevents dying from pressing backspace while paused)
  - InstantComplete (Triggers winning the level while the module is on, can be used as verify hack)
  - CheckpointFix (Tries to fix practice mode inconsitencies)
  - PracticeMusic (Adds the normal music to practice mode)
  - KeepCheckpoints (Does not remove checkpoints when entering/exiting practice mode, useful when editing levels)
- Visual
  - CheatIndicator (Displays a red dot whenever any cheats are enabled)
  - ForcePlayerCamera (Forces the camera to look at the player)
  - HidePauseMenu (Make the pause menu invisible and unclickable)
  - ShowHitbosex (Shows the hitboxes of hazards, the player and other objects)
  - HitboxesOnDeath (Shows hitboxes after you die)
  - NoObjectFade (Makes objects fade in instantly instead of slowly growing)
  - RenderFullLevel (Loads the whole level at the same time. Can be laggy)
- Shortcuts (These allow you to quickly navigate to different menus of the game quickly)
  - Main Menu
  - Level Editor
  - Online Levels
  - Offline Levels
  - Online Level by ID
  - Quit Game
- Replays (These are buggy sometimes, unfortunately)
  - Allows you to record and play replays.
  - Currently for best results it's recommended to enable `CheckpointFix` and `TargetFPS` with `Lock Delta` enabled
  - Also make sure `VSync` is disabled, as it can mess with the `Lock Delta` Option
- Misc
  - Fullscreen (Allows you to disable fullscreen and change the window resolution)
  - Jumpscare (Randomly jumpscares you when you die)
  - TargetFPS (Allows you to set a target framerate for the game to run at)
  - VSync (Allows you to enable VSync which locks the game's framerate to your monitor's refresh rate) 

##### Potentially outdated image of the mod menu.
![pic of mod menu](https://github.com/art0007i/3DashUtils/assets/19620451/814305d6-3164-4b32-8391-a1313dbe806e)

## Lore

https://github.com/art0007i/3DashUtils/assets/19620451/da63cba6-7f68-4e68-8fd2-7ec2bba9f7a5

## Building

1. Add a valid GamePath to the [.csproj](https://github.com/art0007i/3DashUtils/blob/0c616d8580f95afa18ef80246eb27f41a7cc2ecd/_3DashUtils.csproj#L13) file
1. use dotnet cli or visual studio or whatever else u use to compile code
