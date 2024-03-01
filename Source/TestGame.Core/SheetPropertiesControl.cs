using ImGuiNET;

namespace Tools 
{
  public partial class SpriteSheetEditor 
  {
    public partial class SheetPropertiesControl : Control 
    {
      public override void Render() 
      {  
        if (Editor.SpriteSheet == null) return;
        // if (ImGui.IsWindowHovered() && Editor.EditState == EditingState.INACTIVE) Editor.Set(EditingState.ACTIVE);
        DrawSheetProps(Editor.SpriteSheet, Editor.SpriteSheet.Name);
        if (Gui.Selection == null) return;
        if (Gui.Selection is TiledSpriteData tiledSprite) DrawPropertiesPane(tiledSprite, tiledSprite.Name);
        else if (Gui.Selection is ComplexSpriteData complexSprite) DrawPropertiesPane(complexSprite, complexSprite.Name);

      }
      void DrawPropertiesPane(ComplexSpriteData sprite, string name)
      {
        ImGui.Begin(Names.ObjectPropertiesPane);
        ImUtils.LabelText("Name", name);
        ImGui.SeparatorText("Custom Properties");
        foreach (var (customName, customProp) in sprite.Properties) 
        {
          ImUtils.LabelText(customName, "-");
        }
        ImGui.End();
      }
      void DrawPropertiesPane(TiledSpriteData sprite, string name)
      {
        ImGui.Begin(Names.ObjectPropertiesPane);
        // ImGui.PushFont(_font);
        ImUtils.LabelText("Name", name);
        ImUtils.LabelText("Region", sprite.Region.RenderStringFormat());
        if (ImGui.MenuItem("Create ComplexSprite")) Editor.SpriteSheet.AddSprite(sprite);
        ImGui.SeparatorText("Custom Properties");
        foreach (var (customName, customProp) in sprite.Properties) 
        {
          ImUtils.LabelText(customName, "-");
        }
        ImGui.End();
      }
      void DrawSheetProps(SpriteSheetData spriteSheet, string name)
      {
        ImGui.Begin(Names.SheetPropertiesPane);
        // ImGui.PushFont(Gui.NormalFont);
        ImUtils.LabelText("Name", name);
        ImUtils.LabelText("Tile width", $"{spriteSheet.TileWidth}");
        ImUtils.LabelText("Tile height", $"{spriteSheet.TileHeight}");
        ImUtils.LabelText("EditState", $"{Editor.EditState}");
        // ImGui.PopFont();
        ImGui.End();

      }
    }
  }
}
