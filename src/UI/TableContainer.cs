using Godot;
using Godot.Collections;
using System.Linq;
using Label = Godot.Label;

[Tool]
public partial class TableContainer : VBoxContainer
{
    public Array<TableHeader> Headers { set; get; } = new();
    public Array<Array<Variant>> Rows = new();

    [Export]
    public HBoxContainer HeaderContainer;
    [Export]
    public HBoxContainer RowContainer;

    public float lastW = 0;


    public void AddHeader(string text, float ratio = 0.0f)
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

    public void ClearRows()
    {
        Rows.Clear();
        UpdateDisplayRows();
    }



    public void UpdateDisplayHeader()
    {
        var sepColor = GetThemeColor("base_color", "Editor");

        // Clear old children
        foreach (var c in HeaderContainer.GetChildren())
            c.QueueFree();
        foreach (var c in RowContainer.GetChildren())
            c.QueueFree();

        int i = 0;
        // Add new ones
        foreach (var h in Headers)
        {
            // Label
            var label = new Label
            {
                Text = h.Text,
                SizeFlagsHorizontal = SizeFlags.Fill | SizeFlags.Expand,
                ClipText = true,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            HeaderContainer.AddChild(label);
            h.HeaderControl = label;

            // Separator
            if (i < Headers.Count - 1)
            {
                var sepContainer = new HBoxContainer()
                {
                    CustomMinimumSize = new Vector2(8, 1),
                    MouseDefaultCursorShape = CursorShape.Hsize,
                    SizeFlagsVertical = SizeFlags.Fill | SizeFlags.Expand,
                    Alignment = AlignmentMode.Center
                };
                sepContainer.AddThemeConstantOverride("separation", 0);
                var sep = new ColorRect
                {
                    Color = sepColor,
                    CustomMinimumSize = new Vector2(2, 1),
                    MouseFilter = MouseFilterEnum.Ignore
                };
                sepContainer.AddChild(sep);
                HeaderContainer.AddChild(sepContainer);
            }

            // Add the rows columns
            var col = new VBoxContainer
            {
                SizeFlagsHorizontal = SizeFlags.Fill | SizeFlags.Expand,
                ClipChildren = ClipChildrenMode.Only
            };
            RowContainer.AddChild(col);
            h.RowControl = col;
            // Row Separators
            if (i < Headers.Count - 1)
            {
                var sepContainer = new HBoxContainer()
                {
                    CustomMinimumSize = new Vector2(8, 1),
                    SizeFlagsVertical = SizeFlags.Fill | SizeFlags.Expand,
                    Alignment = AlignmentMode.Center
                };
                sepContainer.AddThemeConstantOverride("separation", 0);
                var sep = new ColorRect
                {
                    Color = sepColor,
                    CustomMinimumSize = new Vector2(2, 1)
                };
                sepContainer.AddChild(sep);
                RowContainer.AddChild(sepContainer);
            }

            i += 1;
        }

        // Recalculate minimum recommended width
        CallDeferred(MethodName.RecalculateRecommendedWidthHeaders);

    }

    public void RecalculateRecommendedWidthHeaders()
    {
        foreach (var header in Headers)
        {
            header.RecalculateRecommendedWidth();
            header.Width = header.RecommendedWidth;
        }
        UpdateColumnsWidth();
    }

    public void UpdateDisplayRows()
    {
        // Clear old children
        foreach (var h in Headers)
        {
            foreach(var c in h.RowControl.GetChildren())
                c.QueueFree();
        }

        UpdateColumnsWidth(true);

        // Add new ones
        foreach (var row in Rows)
        {
            int hind = 0;
            foreach (var h in Headers)
            {
                // Add data from each headers
                var label = new Label();
                label.Text = row[hind].AsString();
                label.ClipText = true;
                label.SizeFlagsHorizontal = SizeFlags.Fill | SizeFlags.Expand;
                h.RowControl.AddChild(label);

                hind++;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (IsVisibleInTree())
        {
            UpdateColumnsWidth();
        }
    }

    public void UpdateColumnsWidth(bool force=false)
    {
        // Set the size to match the headers size
        var contW = HeaderContainer.Size.X;

        if (lastW != contW || force)
        {
            var r = 1f;
            var i = 0;
            foreach (var header in Headers)
            {
                var widthUsed = 0;
                if (header.Ratio != 1)
                {
                    header.HeaderControl.SizeFlagsStretchRatio = header.Width / contW;
                    header.RowControl.SizeFlagsStretchRatio = header.Width / contW;

                    widthUsed += header.Width;
                }

                if (i < Headers.Count - 1)
                {
                    // Add the separator width
                    widthUsed += 8;
                }

                r -= widthUsed / contW;

                i++;
            }


            // Add the container separation value
            var sep = GetThemeConstant("separation");
            r -= (sep * (HeaderContainer.GetChildCount()-1)) / contW;

            float r1Total = r / Headers.Where((head,i) => { return head.Ratio == 1; }).Count();
            foreach (var header in Headers)
            {
                if (header.Ratio == 1)
                {
                    header.HeaderControl.SizeFlagsStretchRatio = r1Total;
                    header.RowControl.SizeFlagsStretchRatio = r1Total;
                }
            }
            lastW = contW;
        }
    }

    public void UpdateDisplay()
    {
        UpdateDisplayHeader();
        UpdateDisplayRows();
        CallDeferred(MethodName.UpdateColumnsWidth, true);
    }
}
