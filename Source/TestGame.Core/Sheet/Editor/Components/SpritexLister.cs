using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ImGuiTools;
using Num = System.Numerics;


namespace Raven.Sheet
{
  public class SpritexLister : Editor.SubEntity
  {
    public override void OnAddedToScene() => Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);
    public void RenderImGui() 
    {
      if (Editor.SpriteSheet == null) return;
      ImGui.Begin(GetType().Name);
      if (ImGui.CollapsingHeader($"Tiles ({Editor.SpriteSheet.Tiles.X * Editor.SpriteSheet.Tiles.Y})"))
      {
        foreach (var (name, tile) in Editor.SpriteSheet.TileMap)
        {
          ImGui.Indent();
          if (ImGui.MenuItem($"{name}"))
          {
          }
          ImGui.Unindent();
        }
      }
      if (ImGui.CollapsingHeader($"Spritexes ({Editor.SpriteSheet.Spritexes.Count})"))
      {
        ImGui.Indent();
        foreach (var (name, spritex) in Editor.SpriteSheet.Spritexes)
        {
          if (ImGui.MenuItem($"{name}")) 
          {
            var spritexView = Editor.GetSubEntity<SpritexView>();
            spritexView.Edit(spritex);
          }
        }
        ImGui.Unindent();
        ImGui.Separator();
      }
      ImGui.End();
    }
  }
}

