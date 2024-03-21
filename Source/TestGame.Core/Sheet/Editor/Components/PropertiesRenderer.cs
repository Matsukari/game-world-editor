using ImGuiNET;
using Nez;
using Nez.ImGuiTools;
using Microsoft.Xna.Framework;

namespace Raven.Sheet
{
  public class PropertiesRenderer : Editor.SubEntity
  {
    public override void OnAddedToScene() 
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);    
      var render = AddComponent(new Renderable());
      render.RenderLayer = -1;
    }
    public void RenderImGui() 
    { 
      Editor.RenderImGui(this);
      if (Gui.Selection is IPropertied propertied) propertied.RenderImGui(this);  
    }
    public class Renderable : Editor.SubEntity.RenderableComponent<PropertiesRenderer>
    {
      public override void Render(Batcher batcher, Camera camera)
      {
        if (Gui.ShapeContext != null)
        {
          // Draw annotated shapes
          Gui.primitiveBatch.Begin(camera.ProjectionMatrix, camera.TransformMatrix);
          Annotator.Renderable.DrawPropertiesShapes(Gui.ShapeContext, Gui.primitiveBatch, batcher, camera, Editor.ColorSet.AnnotatedShapeInactive);
          Gui.primitiveBatch.End();

          // Check selection of shapes
          var selectionRect = Editor.GetSubEntity<Selection>();
          foreach (var shape in Gui.ShapeContext.Properties)
          {
            if (Nez.Input.LeftMouseButtonPressed 
                && shape.Value is Shape propShape 
                && propShape.Bounds.Contains(Entity.Scene.Camera.MouseToWorldPoint()))
            {
              selectionRect.Begin(propShape.Bounds, propShape);

              // Remove any tiles selected; aren't of significance
              if (Editor.GetSubEntity<SheetView>().Enabled) 
                Editor.GetSubEntity<SheetSelector>().RemoveSelection();
              break;
            }
          } 
          // update selection
          if (selectionRect.Capture is Shape theShape)
          {
            theShape.Bounds = selectionRect.Bounds;
          }
          DrawNames(batcher, camera);
        }
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
        if (Editor.GetCurrent() is Sheet sheet && Editor.GetSubEntity<SheetView>().Enabled)
        {
          foreach (var tile in sheet.TileMap)
          {
            DrawText(tile.Value.Name, Editor.GetSubEntity<SheetView>().GetRegionInSheet(tile.Value.Region.ToRectangleF()).BottomLeft(), 
                2.5f/tile.Value.Name.Count());
          }   
        }
        // Draw spritex parts names
        else if (Editor.GetSubEntity<SpritexView>().Enabled && Gui.Selection is Sprites.Spritex spritex)
        {
          foreach (var part in spritex.Body)
          {
            DrawText(part.Name, part.WorldBounds.BottomCenter(), Math.Clamp(1f/camera.RawZoom, 1f, 10f));
          }
        }
      }
    }
  }
}
