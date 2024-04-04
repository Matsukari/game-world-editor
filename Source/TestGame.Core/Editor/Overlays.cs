using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{
  public class Overlays
  {
    readonly EditorSettings _settings;
    readonly Selection _selection;

    public Overlays() {}

    public void Render(Batcher batcher, Camera camera)
    {
      ShapeAnnotator.DrawPropertiesShapes(ContentData.ShapeContext, batcher, camera, Editor.Settings.Colors.ShapeInactive.ToColor());

      // Check selection of shapes
      var selectionRect = Editor.GetEditorComponent<Selection>();
      foreach (var shape in ContentData.ShapeContext.Properties)
      {
        if (Nez.Input.LeftMouseButtonPressed 
            && shape.Value is Shape propShape 
            && propShape.Bounds.Contains(Entity.Scene.Camera.MouseToWorldPoint()))
        {
          selectionRect.Begin(propShape.Bounds, propShape);

          // Remove any tiles selected; aren't of significance
          if (Editor.GetEditorComponent<SheetView>().Enabled) 
            Editor.GetEditorComponent<SheetSelector>().RemoveSelection();
          break;
        }
        // update selection
        if (selectionRect.Capture is Shape theShape)
        {
          theShape.Bounds = selectionRect.ContentBounds;
        }
      }
      DrawNames(batcher, camera);
    }
    void DrawNames(Batcher batcher, Camera camera)
    {
      void DrawText(string text, Vector2 pos, float scale)
      {
        batcher.DrawString(
            Graphics.Instance.BitmapFont, 
            text,
            pos,
            color: Color.DarkGray, 
            rotation: 0f, 
            origin: Vector2.Zero, 
            scale: scale, 
            effects: Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 
            layerDepth: 0f);
      }
      // Draw created tiles' names
      if (Content is Sheet sheet && Editor.GetEditorComponent<SheetView>().Enabled)
      {
        foreach (var tile in sheet.TileMap)
        {
          DrawText(tile.Value.Name, Editor.GetEditorComponent<SheetView>().GetRegionInSheet(tile.Value.Region.ToRectangleF()).BottomLeft(), 
              2.5f/tile.Value.Name.Count());
        }   
      }
    }
  }
}
