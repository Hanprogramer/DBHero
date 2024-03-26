using Godot;
using Godot.Collections;
[Tool]
public partial class EntryController : PanelContainer
{
	[Export]
	public TableContainer MainTable;
    private DBHDatabase Database;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void SetDisplay(string resourcePath)
    {
        var db = GD.Load<DBHDatabase>(resourcePath);
        if (db is DBHDatabase dbh)
        {
            Database = dbh;
            MainTable.ClearRows();
            MainTable.ClearHeaders();
            foreach (var structure in dbh.Structure)
            {
                MainTable.AddHeader(structure.FieldName);
            }
            foreach (var data in dbh.Entries)
            {
                MainTable.AddRow((Array<Variant>)data.Values.Values);
            }

            MainTable.CallDeferred(TableContainer.MethodName.UpdateDisplay);
        }
        else
        {
            GD.PrintErr($"Can't open DBH Database, not a database: {resourcePath}, it's a {db}");
        }
    }
}
