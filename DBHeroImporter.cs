#if TOOLS
using Godot;
using Godot.Collections;
using System.Text;

[Tool]
public partial class DBHeroImporter : EditorImportPlugin
{
    public override string _GetImporterName()
    {
        return "hanprogramer.dbhero";
    }

    public override Array<Dictionary> _GetImportOptions(string path, int presetIndex)
    {
        return [
            // DB Name
            new(){
                { "name" , "db_name" },
                { "default_value", "" },
                { "hint_string", "MyDB" },
                { "property_hint", (int)PropertyHint.PlaceholderText }
            },
            // Namespace
            new(){
                { "name" , "namespace" },
                { "default_value", "" },
                { "hint_string", "MyGame" },
                { "property_hint", (int)PropertyHint.PlaceholderText }
            },
            // Generate class?
            new(){
                { "name" , "generate_class" },
                { "hint_string", "MyDB" },
                { "default_value", true },
            },
            // Class Name
            new(){
                { "name" , "class_name" },
                { "default_value", "" },
                { "hint_string", "MyClass" },
                { "property_hint", (int)PropertyHint.PlaceholderText }
            },
        ];
    }

    public override int _GetImportOrder()
    {
        return 0;
    }

    public override bool _GetOptionVisibility(string path, StringName optionName, Dictionary options)
    {
        return true;
    }

    public override int _GetPresetCount()
    {
        return 0;
    }

    public override string _GetPresetName(int presetIndex)
    {
        return base._GetPresetName(presetIndex);
    }

    public override float _GetPriority()
    {
        return 1.0f;
    }



    public override string[] _GetRecognizedExtensions()
    {
        return ["dbhero"];
    }

    public override string _GetResourceType()
    {
        return "DBHeroDB";
    }

    public override string _GetSaveExtension()
    {
        return "res";
    }

    public override string _GetVisibleName()
    {
        return "DBHero Database";
    }

    public override Error _Import(string sourceFile, string savePath, Dictionary options, Array<string> platformVariants, Array<string> genFiles)
    {
        var dbName = (string)options["db_name"];
        var classNameSpace = ((string)options["namespace"]).Trim();
        var className = (string)options["class_name"];

        var res = new DBHDatabase(className, dbName, classNameSpace);

        // Load the db and parse it
        res.LoadFromXML(sourceFile);

        if ((bool)options["generate_class"])
        {
            // Generate C# Script if asked

            var content = res.GenerateCSharp();

            // Write the script
            using var file = FileAccess.Open($"{sourceFile}.generated.cs", FileAccess.ModeFlags.Write);
            file.StoreString(content);
            file.Close();
        }
        return ResourceSaver.Singleton.Save(res, $"{savePath}.{_GetSaveExtension()}");
    }
}
#endif
