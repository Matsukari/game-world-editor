using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ImGuiTools;
using Num = System.Numerics;


namespace Raven.Sheet
{
  public class SpritexLister : Editor.SheetEntity
  {
    public override void OnAddedToScene() => Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);
    public void RenderImGui() 
    {
      if (Sheet == null) return;
      ImGui.Begin(IconFonts.FontAwesome5.List + " " + GetType().Name);
      if (ImGui.CollapsingHeader($"{IconFonts.FontAwesome5.Th} Tiles ({Sheet.Tiles.X * Sheet.Tiles.Y})"))
      {
        foreach (var (name, tile) in Sheet.TileMap)
        {
          ImGui.Indent();
          if (ImGui.MenuItem($"{name}"))
          {
          }
          ImGui.Unindent();
        }
      }
      if (ImGui.CollapsingHeader($"{IconFonts.FontAwesome5.Users} Spritexes ({Sheet.Spritexes.Count})", ImGuiTreeNodeFlags.DefaultOpen))
      {
        ImGui.BeginChild("spritexes");
        ImGui.Indent();
        foreach (var (name, spritex) in Sheet.Spritexes)
        {
          if (ImGui.MenuItem($"{IconFonts.FontAwesome5.User} {name}")) 
          {
            var spritexView = Editor.GetSubEntity<SpritexView>();
            spritexView.Edit(spritex);
          }
        }
        ImGui.Unindent();
        ImGui.EndChild();
      }
      ImGui.End();
    }
  }
}

