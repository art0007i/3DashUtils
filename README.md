# 3DashUtils

A [BepInEx](https://github.com/BepInEx/BepInEx) 5.x plugin for [3Dash](https://delugedrop.itch.io/3dash)

It adds a menu (toggled by pressing tab) which gives you access to a bunch of different utilities and fixes.

## Installation

1. Download [BepInEx](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.21)
2. Extract the downloaded zip file on top of your game (the BepInEx folder should be in the same folder as 3Dash.exe)
3. Download [3DashUtils](https://github.com/art0007i/3DashUtils/releases/latest/)
4. Extract the downloaded zip file into the `BepInEx/plugins` directory. If it doesn't exist you may need to create it yourself.
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

## Features

![pic of mod menu](https://github.com/art0007i/3DashUtils/assets/19620451/f147837a-b249-428d-b998-dc1ed559734a)


## Building

1. Add a valid GamePath to the [.csproj](https://github.com/art0007i/3DashUtils/blob/63a246e0a144f0cefdbf4cc5c513aafd7370e87a/_3DashUtils.csproj#L12) file
1. use dotnet cli or visual studio or whatever else u use to compile code
