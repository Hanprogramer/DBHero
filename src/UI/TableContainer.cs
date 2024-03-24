using Godot;
using Godot.Collections;

[Tool]
public partial class TableContainer : VBoxContainer
{
    public Array<TableHeader> Headers = new();
    public Array<Array<Variant>> Rows = new();

    [Export]
    public HBoxContainer HeaderContainer;
    [Export]
    public VBoxContainer RowContainer;

    public void AddHeader(string text, float ratio = 1.0f)
    {
        Headers.Add(new TableHeader()
        {
            Text = text,
            Ratio = ratio
        });
        UpdateDisplayHeader();
    }

    public void AddRow(Array<Variant> data)
    {
        Rows.Add(data);
        UpdateDisplayRows();
    }

    public void ClearHeaders(bool updateDisplay = false)
    {
        Headers.Clear();
        if (updateDisplay)
            UpdateDisplayHeader();
    }

    public void UpdateDisplayHeader()
    {
        // Clear old children
        foreach (var c in HeaderContainer.GetChildren())
        {
            c.QueueFree();
        }

        // Add new ones
        foreach (var h in Headers)
        {
            var label = new Label();
            label.Text = h.Text;
            label.SizeFlagsHorizontal = SizeFlags.Fill | SizeFlags.Expand;
            label.SizeFlagsStretchRatio = h.Ratio;
            HeaderContainer.AddChild(label);
        }
    }

    public void UpdateDisplayRows()
    {
        // Clear old children
        foreach (var c in RowContainer.GetChildren())
        {
            c.QueueFree();
        }

        // Add new ones
        foreach (var row in Rows)
        {
            var hb = new HBoxContainer();

            int i = 0;
            foreach (var data in row)
            {
                var label = new Label();
                label.Text = data.AsString();
                label.SizeFlagsHorizontal = SizeFlags.Fill | SizeFlags.Expand;
                label.SizeFlagsStretchRatio = Headers[i].Ratio;
                hb.AddChild(label);
                i++;
            }

            RowContainer.AddChild(hb);
        }
    }
}
