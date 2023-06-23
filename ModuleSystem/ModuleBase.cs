namespace _3DashUtils.ModuleSystem;

/// <summary>
/// The class that you should inherit when making a new module.
/// Unless you are making a toggle module in which case you should inherit <see cref="ToggleModule{T}"/>
/// </summary>
/// <typeparam name="T">This should be a self-referencing paramater. So for example <c>Noclip : ModuleBase&lt;Noclip&gt;</c></typeparam>
public abstract class ModuleBase : IMenuModule
{
    public abstract string CategoryName { get; }

    private string cachedName;

    public ModuleBase()
    {
        // for the lazy developer :3
        cachedName = Extensions.SplitPascalCase(this.GetType().Name);
    }

    public virtual string ModuleName => cachedName;

    public virtual float Priority => 0;

    public virtual bool IsCheat => false;

    public virtual void Awake()
    {
    }

    public virtual void OnGUI()
    {
    }

    public virtual void OnUnityGUI()
    {
    }

    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }
    public virtual void FixedUpdate()
    {
    }
    public virtual void LateUpdate()
    {
    }
}
