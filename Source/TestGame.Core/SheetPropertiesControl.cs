using ImGuiNET;
using Nez;

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
        ImGui.Indent();
        ImUtils.LabelText("Name", name);
        ImUtils.LabelText("Type", "Complex");
        ImGui.Unindent();
        ImGui.Separator();
        DrawCustomProperties(sprite.Properties);
        ImGui.End();
      }
      void DrawPropertiesPane(TiledSpriteData sprite, string name)
      {
        ImGui.Begin(Names.ObjectPropertiesPane);
        ImGui.Indent();
        ImUtils.LabelText("Name", name);
        ImUtils.LabelText("Type", "Tiled");
        ImUtils.LabelText("Region", sprite.Region.RenderStringFormat());
        ImGui.Unindent();
        ImGui.Separator();
        ImGui.Indent();
        if (ImGui.MenuItem("Complex")) Editor.SpriteSheet.AddSprite(sprite);
        if (ImGui.MenuItem("Delete")) Editor.SpriteSheet.Delete(sprite);
        if (ImGui.MenuItem("Combine")) 
        {
          var newSprite = Editor.SpriteSheet.CombineContains(sprite, Gui.SelectionRect.Content);
          // if (newSprite.Region != RectangleF.Empty) Editor.GetComponent<SheetImageControl>().Select(newSprite);
          
        }
        ImGui.Unindent();
        ImGui.Separator();
        DrawCustomProperties(sprite.Properties);
        ImGui.End();
      }
      void DrawSheetProps(SpriteSheetData spriteSheet, string name)
      {
        ImGui.Begin(Names.SheetPropertiesPane);
        ImGui.Indent();
        ImUtils.LabelText("Name", name);
        ImUtils.LabelText("Tile width", $"{spriteSheet.TileWidth}");
        ImUtils.LabelText("Tile height", $"{spriteSheet.TileHeight}");
        ImUtils.LabelText("EditState", $"{Editor.EditState}");
        ImGui.Unindent(); 
        DrawCustomProperties(spriteSheet.Properties);
        ImGui.End();
      }
      void DrawCustomProperties(CustomProperties props)
      {
        ImGui.SeparatorText("Custom Properties");
        ImGui.Indent(); 
        string deleteId = string.Empty;
        foreach (var (customName, customProp) in props) 
        {
          if (ImGui.CollapsingHeader(customName))
          {
            if (customProp is Shape.Rectangle r) ImGui.Text(r.Bounds.RenderStringFormat());
            if (ImGui.MenuItem("Delete")) deleteId = customName;
          }
        }
        if (deleteId != string.Empty) props.Remove(deleteId);

        ImGui.Unindent();
      }
    }
  }
}
