#if TOOLS
using Godot;
using Godot.Collections;
using System.Text;

[Tool]
public partial class DBHeroImporter : EditorImportPlugin
{
    //public CSharpScript DBHDBScript = GD.Load<CSharpScript>("res://addons/DBHero/src/Scripts/DBHDatabase.cs");

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
        return 1;
    }

    public override string _GetPresetName(int presetIndex)
    {
        return "Default";
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
        return "DBHDatabase";
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

        // TODO: add the last parameter here to the import settings
        CSharpScript DBHDBScript = GD.Load<CSharpScript>("res://addons/DBHero/src/Scripts/DBHDatabase.cs");
        var res = (DBHDatabase)DBHDBScript.New(className, dbName, classNameSpace, true);
        //var res =  new DBHDatabase(className, dbName, classNameSpace);

        // Load the db and parse it
        if (res.LoadFromXML(sourceFile) != Error.Ok)
        {
            GD.PrintErr($"Error importing {sourceFile}, invalid XML.");
            return Error.Failed;
        }

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
