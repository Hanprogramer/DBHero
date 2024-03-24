#if TOOLS
using Godot;

[Tool]
public partial class DBHero : EditorPlugin
{
    PackedScene EditorScene = GD.Load<PackedScene>("res://addons/DBHero/src/scenes/DBHeroEditor.tscn");
    Control Editor;

    public override void _EnterTree()
    {
        Editor = EditorScene.Instantiate<Control>();
        EditorInterface.Singleton.GetEditorMainScreen().AddChild(Editor);
        Editor.Visible = false;

        AddCustomType("DBHDatabase", "Resource", GD.Load<CSharpScript>("res://addons/DBHero/src/Scripts/DBHDatabase.cs"), GD.Load<Texture2D>("res://icon.svg"));

        AddImportPlugin(new DBHeroImporter());
        ResourceLoader.AddResourceFormatLoader(new DBHLoader(), true);
        //ResourceLoader.(new DBHLoader());

    }

    public override void _ExitTree()
    {
        Editor?.QueueFree();
    }

    public override Texture2D _GetPluginIcon()
    {
        return base._GetPluginIcon();
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
