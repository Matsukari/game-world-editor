using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;

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
      }
      void DrawPropertiesPane(TiledSpriteData sprite, string name)
      {
        ImGui.Begin(Names.ObjectPropertiesPane);
        ImGui.Indent();
        if (ImGui.InputText("Name", ref name, 12)) sprite.Name = name;
        ImGui.LabelText("Type", "Tiled");
        ImGui.LabelText("Region", sprite.Region.RenderStringFormat());
        ImGui.Unindent();
        ImGui.Separator();
        ImGui.NewLine();
        ImGui.Indent();
        if (ImGui.MenuItem("Complex")) Editor.SpriteSheet.AddSprite(sprite);
        if (ImGui.MenuItem("Combine")) 
        {
          var newSprite = Editor.SpriteSheet.CombineContains(sprite, Gui.SelectionRect.Content);
          if (newSprite.Region != RectangleF.Empty) Editor.GetComponent<SheetImageControl>().Select(newSprite);
          
        }
        ImGui.PushStyleColor(ImGuiCol.Button, Editor.ColorSet.DeleteButton.ToImColor());
        if (ImGui.Button("Delete")) Editor.SpriteSheet.Delete(sprite); 
        ImGui.PopStyleColor();
        ImGui.Unindent();
        ImGui.NewLine();
        ImGui.Separator();
        DrawCustomProperties(sprite.Properties, Editor);
        ImGui.End();
      }
      void DrawSheetProps(SpriteSheetData spriteSheet, string name)
      {
        ImGui.Begin(Names.SheetPropertiesPane);
        ImGui.Indent();
        if (ImGui.InputText("Name", ref name, 12)) spriteSheet.Name = name;
        ImGui.BeginDisabled();
        ImGui.InputInt("Block_Width", ref spriteSheet.TileWidth);
        ImGui.InputInt("Block_Height", ref spriteSheet.TileHeight);
        ImGui.EndDisabled();
        ImGui.TextDisabled($"EditState: {Editor.EditState}");
        ImGui.NewLine();
        ImGui.Unindent(); 
        DrawCustomProperties(spriteSheet.Properties, Editor);
        ImGui.End();
      }
      public static void DrawCustomProperties(CustomProperties props, SpriteSheetEditor editor)
      {
        ImGui.SeparatorText("Properties");
        string deleteId = string.Empty;
        foreach (var (customName, customProp) in props) 
        {
          if (ImGui.MenuItem(customName))
          {
            ImGui.Indent(); 
            var bounds = new System.Numerics.Vector4();
            if (customProp is Shape shape && ImGui.InputFloat4("Bounds", ref bounds)) shape.Bounds = bounds.ToRectangleF();
            ImGui.PushStyleColor(ImGuiCol.Button, editor.ColorSet.DeleteButton.ToImColor());
            if (ImGui.Button("Delete")) deleteId = customName;
            ImGui.PopStyleColor();

            if (ImGui.ColorButton("Delete", editor.ColorSet.DeleteButton.ToVector4().ToNumerics())) deleteId = customName;
            ImGui.Unindent();
          }
        }
        if (deleteId != string.Empty) props.Remove(deleteId);

      }
    }
  }
}
