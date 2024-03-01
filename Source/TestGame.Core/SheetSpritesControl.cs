using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;
using Num = System.Numerics;


namespace Tools 
{
  public partial class SpriteSheetEditor 
  {
    public partial class SheetSpritesControl : Control 
    {
      public override void Render() 
      {
        if (Editor.SpriteSheet == null) return;
        ImGui.Begin("Sprite List");
        ImGui.Button($"Sheet ({Editor.SpriteSheet.Tiles.Count})");
        ImGui.Separator();
        ImGui.Button($"Sprites ({Editor.SpriteSheet.Sprites.Count})");
        ImGui.Separator();
        ImGui.Indent();
        foreach (var (name, sprite) in Editor.SpriteSheet.Sprites)
        {
          if (ImGui.MenuItem($"{name}")) 
          {
            Editor.GetComponent<SheetImageControl>().Select(sprite);
          }
        }
        ImGui.Unindent();
        ImGui.End();
      }
    }
  }
}

