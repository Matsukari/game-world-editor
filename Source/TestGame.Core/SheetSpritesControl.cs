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
        ImGui.TextDisabled($"Sheet ({Editor.SpriteSheet.Tiles.Count})");
        ImGui.Separator();
        ImGui.TextDisabled($"Sprites ({Editor.SpriteSheet.Sprites.Count})");
        ImGui.Separator();
        ImGui.Indent();
        foreach (var (name, sprite) in Editor.SpriteSheet.Sprites)
        {
          if (ImGui.Button($"{name}")) 
          {
            Editor.GetComponent<SheetImageControl>().Select(sprite);
            var complex = Editor.Entity.Scene.FindEntity(Names.ComplexSprite) as ComplexSpriteEntity;
            complex.Edit(sprite);
          }
        }
        ImGui.Unindent();
        ImGui.End();
      }
    }
  }
}

