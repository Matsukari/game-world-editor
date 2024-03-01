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
        ImGui.SeparatorText($"Sheet ({Editor.SpriteSheet.Tiles.Count})");
        ImGui.SeparatorText($"Sprites ({Editor.SpriteSheet.Sprites.Count})");
        foreach (var (name, sprite) in Editor.SpriteSheet.Sprites)
        {
          if (ImGui.MenuItem($"{name}")) 
          {

          }
        }
        ImGui.End();
      }
    }
  }
}

