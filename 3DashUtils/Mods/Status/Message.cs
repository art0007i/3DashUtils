using System.Security.Permissions;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using UnityEngine;

namespace _3DashUtils.Mods.Status;
public class Message : TemplateLabel
{
    public override string CategoryName => "Status";

    public override string ModuleName => "Message";

    protected override bool Default => false;

    public override string text => MessageField;

    public override float Priority => -2;    

    public static ConfigOptionBase<string> messageField;
    public static string MessageField => messageField.Value;

    public Message()
    {
        messageField = new TextInputConfig<string>(this, "Message", "", "The message to display in the top left corner");
    }
}

