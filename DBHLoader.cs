
using Godot;
[Tool]
public partial class DBHLoader : ResourceFormatLoader
{
    public override string[] _GetRecognizedExtensions()
    {
        return ["dbhero"];
    }

    public override string _GetResourceType(string path)
    {
        return "DBHDatabase";
    }

    public override bool _HandlesType(StringName type)
    {
        return ClassDB.IsParentClass(type, "DBHDatabase");
    }

    public override Variant _Load(string path, string originalPath, bool useSubThreads, int cacheMode)
    {
        var dbh = new DBHDatabase();
        dbh.LoadFromXML(path);
        return dbh;
    }

}
