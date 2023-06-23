using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3DashUtils.ModuleSystem;

/// <summary>
/// Class that defines a few things necessary to display a keybind editing modal.
/// Intended to instantiated within modules and written to <see cref="_3DashUtils.currentKeybindEditing"/> to display the modal.
/// </summary>
public class KeyBindEditInfo
{
    /// <summary>
    /// The default key that is used when resetting the keybind.
    /// </summary>
    public KeyCode defaultKey;
    /// <summary>
    /// Will be called whenever the user selects a new keybind. Does not get called if the user cancels editing.
    /// </summary>
    public Action<KeyCode> callback;
    /// <summary>
    /// Name of the keybind. Will be shown in a format like
    /// <code>Enter a new key for {keyBindName}</code>
    /// </summary>
    public string keyBindName;
    /// <summary>
    /// True after the user has selected a key.
    /// Used internally to prevent you from activating the keybind right after setting it.
    /// </summary>
    public bool editingFinished;
    public KeyCode selectedKey;

    public KeyBindEditInfo(KeyCode defaultKey, Action<KeyCode> callback, string keyBindName)
    {
        this.defaultKey = defaultKey;
        this.callback = callback;
        this.keyBindName = keyBindName;
    }
}
