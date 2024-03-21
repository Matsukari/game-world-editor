
using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven.Sheet
{
  public class LevelGui : Propertied
  {
    Level _level;
    public bool Selected = false;
    public LevelGui(Level level)
    {
      _level = level;
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.LayerGroup;
    }
    protected override void OnRenderAfterName(PropertiesRenderer renderer)
    {
      ImGui.BeginDisabled();
      ImGui.LabelText("Width", $"{_level.ContentSize.X} px");
      ImGui.LabelText("Height", $"{_level.ContentSize.Y} px");
      ImGui.EndDisabled();
    }
  }
}
