using Godot;
using Godot.Collections;
[Tool]
public partial class StructureControl : PanelContainer
{
    [Export]
    public TableContainer MainTable;

    public DBHDatabase Database;

    public override void _Ready()
    {
        base._Ready();
    }
    public void SetDisplay(string resourcePath)
    {
        var db = GD.Load<DBHDatabase>(resourcePath);
        if (db is DBHDatabase dbh)
        {
            Database = dbh;
            MainTable.ClearHeaders();
            MainTable.AddHeader("Field Name", 1.0f);
            MainTable.AddHeader("Type");
            MainTable.AddHeader("Default Value");
            MainTable.AddHeader("Nullable");
            MainTable.AddHeader("Is ID");
            MainTable.AddHeader("Hint");
            MainTable.AddHeader("Generate in C#");

            foreach (var entry in dbh.Structure)
            {
                Array<Variant> data = [
                    entry.FieldName,
                    entry.FieldType.ToString(),
                    entry.DefaultValue,
                    entry.IsNullable,
                    entry.IsId,
                    entry.Hint,
                    entry.GenerateInCSharp
                ];
                MainTable.AddRow(data) ;
            }
        }
        else
        {
            GD.PrintErr($"Can't open DBH Database, not a database: {resourcePath}, it's a {db}");
        }
    }

}
