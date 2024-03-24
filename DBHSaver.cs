using Godot;

public partial class DBHSaver : ResourceFormatSaver
{
    public override string[] _GetRecognizedExtensions(Resource resource)
    {
        return ["dbhero"];
    }

    public override bool _Recognize(Resource resource)
    {
        return resource is DBHDatabase;
    }

    public override Error _Save(Resource resource, string path, uint flags)
    {
        if (resource is DBHDatabase dbh)
        {
            dbh.SaveToXML(path);
        }
        return Error.Failed;
    }
}
