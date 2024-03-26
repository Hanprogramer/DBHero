
using Godot;
using System;

public partial class TableHeader : GodotObject
{
    public string Text = "";
    public float Ratio = 1.0f;
    public Control HeaderControl = null;
    public Control RowControl = null;
    public int RecommendedWidth = 0;
    public int Width = 0;

    public void RecalculateRecommendedWidth()
    {
        if (HeaderControl == null)
            return;
        if (RowControl == null)
            return;

        // Add a padding of 4
        RecommendedWidth = Math.Max(GetMinWidthFor(HeaderControl), GetMinWidthFor(RowControl)) + 32;
    }

    public int GetMinWidthFor(Control control)
    {
        try
        {
            int maxWidth = (int)control.Size.X;
            if (control is Label label)
            {
                // If label then calculate the size of the text
                maxWidth = Math.Max(maxWidth, (int)label.GetThemeDefaultFont().GetStringSize(label.Text).X);
            }
            foreach (var controlChild in control.GetChildren())
            {
                if (controlChild is Control cControlChild)
                {
                    maxWidth = Math.Max(maxWidth, GetMinWidthFor(cControlChild));
                }
            }
            return maxWidth;
        }
        catch (Exception)
        {
            return 0;
        }
    }
}