using ImGuiNET;

namespace Tools 
{
  public partial class SpriteSheetEditor 
  {
    public partial class SheetPropertiesControl : Control 
    {
      public override void Render() 
      {  
        if (Gui.Selection == null || Editor.SpriteSheet == null) return;

        if (ImGui.IsWindowHovered() && Editor.EditState == EditingState.INACTIVE) Editor.Set(EditingState.ACTIVE);

        DrawPropertiesPane(Editor.SpriteSheet, Editor.SpriteSheet.Name);
        if (Gui.Selection is TiledSpriteData tiledSprite) DrawPropertiesPane(tiledSprite, tiledSprite.Name);
        else if (Gui.Selection is ComplexSpriteData complexSprite) DrawPropertiesPane(complexSprite, complexSprite.Name);

      }
      void DrawPropertiesPane(ComplexSpriteData sprite, string name)
      {
        ImGui.Begin(Names.ObjectPropertiesPane);
        ImGui.SeparatorText("Sprite Properties");
        ImGui.LabelText("Name", name);
        ImGui.SeparatorText("Custom Properties");
        foreach (var (customName, customProp) in sprite.Properties) 
        {
          ImGui.LabelText(customName, "-");
        }
        ImGui.End();
      }
      void DrawPropertiesPane(TiledSpriteData sprite, string name)
      {
        ImGui.Begin(Names.ObjectPropertiesPane);
        // ImGui.PushFont(_font);
        ImGui.SeparatorText("Sprite Properties");
        ImGui.LabelText("Name", name);
        ImGui.LabelText("Region", sprite.Region.RenderStringFormat());
        if (ImGui.MenuItem("Create ComplexSprite")) Editor.SpriteSheet.AddSprite(sprite);
        ImGui.SeparatorText("Custom Properties");
        foreach (var (customName, customProp) in sprite.Properties) 
        {
          ImGui.LabelText(customName, "-");
        }
        ImGui.End();
      }
      void DrawPropertiesPane(SpriteSheetData spriteSheet, string name)
      {
        ImGui.Begin(Names.SheetPropertiesPane);
        ImGui.LabelText("Name", name);

        ImGui.LabelText("Tile width", $"{spriteSheet.TileWidth}");
        ImGui.LabelText("Tile height", $"{spriteSheet.TileHeight}");
        ImGui.LabelText("EditState", $"{Editor.EditState}");
        ImGui.End();

      }
    }
  }
}
