#if TOOLS
using Godot;

[Tool]
public partial class DBHero : EditorPlugin
{
    public const string DBH_VERSION = "0.1-beta";
    PackedScene EditorScene = GD.Load<PackedScene>("res://addons/DBHero/src/scenes/DBHeroEditor.tscn");
    Control Editor;

    public override void _EnterTree()
    {
        Editor = EditorScene.Instantiate<Control>();
        EditorInterface.Singleton.GetEditorMainScreen().AddChild(Editor);
        Editor.Visible = false;

        AddCustomType("DBHDatabase", "Resource", 
            GD.Load<CSharpScript>("res://addons/DBHero/src/Scripts/DBHDatabase.cs"), 
            GD.Load<Texture2D>("res://addons/DBHero/dbhero.svg"));

        AddCustomType("EditorIconTexture", "ImageTexture", GD.Load<GDScript>("res://addons/DBHero/src/Util/editor_icon_texture.gd"), null);


        AddImportPlugin(new DBHeroImporter());
        ResourceLoader.AddResourceFormatLoader(new DBHLoader(), true);

        //CallDeferred(MethodName.AddAutoloadSingleton, "DBHero", "res://addons/DBHero/DBHAutoload.cs");
    }

    public override void _ExitTree()
    {
        Editor?.QueueFree();
        //RemoveAutoloadSingleton("DBHero");
    }

    public override Texture2D _GetPluginIcon()
    {
        return GD.Load<Texture2D>("res://addons/DBHero/dbhero.svg");
    }

    public override string _GetPluginName()
    {
        return "DBHero";
    }

    public override bool _HasMainScreen()
    {
        return true;
    }

    public override void _MakeVisible(bool visible)
    {
        if (Editor != null)
            Editor.Visible = visible;
    }

}
#endif
