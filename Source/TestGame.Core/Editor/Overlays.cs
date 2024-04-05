using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{
  public class Overlays
  {
    readonly EditorSettings _settings;
    readonly Selection _selection;

    public Overlays() 
    {
    }

    public void Render(Batcher batcher, Camera camera, Editor editor)
    {
      Editor.PrimitiveBatch.Begin(projection: camera.ProjectionMatrix, view: camera.ViewProjectionMatrix);
      ShapeModelUtils.RenderShapeModel(editor.ContentManager.ContentData.PropertiedContext, 
          Editor.PrimitiveBatch, batcher, camera, editor.Settings.Colors.ShapeInactive.ToColor());
      Editor.PrimitiveBatch.End();


      // Check selection of shapes
      var selectionRect = editor.Selection;
      foreach (var shape in editor.ContentManager.ContentData.PropertiedContext.Properties)
      {
        if (Nez.Input.LeftMouseButtonPressed 
            && ShapeModelUtils.IsShape(shape.Value)
            && ShapeModelUtils.Collides(shape.Value, camera.MouseToWorldPoint()))
        {
          selectionRect.Begin(ShapeModelUtils.Bounds(shape.Value), ShapeModelUtils.ModelsIcon);

          // Remove any tiles selected; aren't of significance
          // if (Editor.GetEditorComponent<SheetView>().Enabled) 
          //   Editor.GetEditorComponent<SheetSelector>().RemoveSelection();
          break;
        }
        // update selection
        // if (selectionRect.Capture is string[])
        // {
        //   theShape.Bounds = selectionRect.ContentBounds;
        // }
      }
      // DrawNames(batcher, camera);
    }
    // void DrawNames(Batcher batcher, Camera camera)
    // {
    //   void DrawText(string text, Vector2 pos, float scale)
    //   {
    //     batcher.DrawString(
    //         Graphics.Instance.BitmapFont, 
    //         text,
    //         pos,
    //         color: Color.DarkGray, 
    //         rotation: 0f, 
    //         origin: Vector2.Zero, 
    //         scale: scale, 
    //         effects: Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 
    //         layerDepth: 0f);
    //   }
      // Draw created tiles' names
      // if (Content is Sheet sheet && Editor.GetEditorComponent<SheetView>().Enabled)
      // {
      //   foreach (var tile in sheet.TileMap)
      //   {
      //     DrawText(tile.Value.Name, Editor.GetEditorComponent<SheetView>().GetRegionInSheet(tile.Value.Region.ToRectangleF()).BottomLeft(), 
      //         2.5f/tile.Value.Name.Count());
      //   }   
      // }
    }
  }
