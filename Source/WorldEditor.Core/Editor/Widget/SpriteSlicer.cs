using ImGuiNET;
using Microsoft.Xna.Framework;

namespace Raven.Widget
{
  public class SpriteSlicer : OkCancelPopup
  {
    public int Cols { get => Sprite.Region.Width / SplitSize.X; set => SplitSize.X = Sprite.Region.Width / value; }
    public int Rows { get => Sprite.Region.Height / SplitSize.Y; set => SplitSize.Y = Sprite.Region.Height / value; } 
    public Point SplitSize = new Point(16, 16);
    public Point SplitCount { get => new Point(Cols, Rows); }
    public Sprite Sprite;

    protected override void OnDraw()
    {
      ImGuiUtils.TextMiddleX("Slice sprite");

      ImGui.NewLine();
      var count = SplitCount;
      if (ImGui.InputInt("Columns", ref count.X) && count.X != 0) Cols = count.X;
      if (ImGui.InputInt("Rows", ref count.Y) && count.Y != 0) Rows = count.Y;

      var splitSize = SplitSize;
      if (ImGui.InputInt("Split Width", ref splitSize.X)) SplitSize.X = splitSize.X;
      if (ImGui.InputInt("Split Height", ref splitSize.Y)) SplitSize.Y = splitSize.Y;
      ImGui.NewLine();

    }    
  }
}
