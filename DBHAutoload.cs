using Godot;

[Tool]
public partial class DBHAutoload : Node
{
    public static void CreateNewDB(string path, string dbName, string namespacename, bool gen_class, string class_name, bool add_default_fields)
    {
        var content = $"""
            <!-- DBHero {DBHero.DBH_VERSION} -->
                <Db ClassName="{class_name}"
            	    DBName="{dbName}"
            	    Namespace="{namespacename}"
            	    AddDefaultFields="{add_default_fields}"
                    GenClass="{gen_class}">
            	    <Structures>
            	    </Structures>
            	    <Entries>
            	    </Entries>
                </Db>
            """;
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        file.StoreString(content);
        file.Close();
    }
}
