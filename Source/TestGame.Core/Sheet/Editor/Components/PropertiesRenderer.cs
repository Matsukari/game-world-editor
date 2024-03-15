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
      if (Editor.SpriteSheet == null) return;
      Editor.RenderImGui(this);
      if (Gui.Selection is IPropertied propertied) 
      {
        propertied.RenderImGui(this); 
      }
    }
    public class Renderable : Editor.SubEntity.RenderableComponent<PropertiesRenderer>
    {
      public override void Render(Batcher batcher, Camera camera)
      {
        if (Gui.ShapeContext != null)
        {
          Gui.primitiveBatch.Begin(camera.ProjectionMatrix, camera.TransformMatrix);
          Annotator.Renderable.DrawPropertiesShapes(Gui.ShapeContext, Gui.primitiveBatch, batcher, camera, Editor.ColorSet.AnnotatedShapeInactive);
          Gui.primitiveBatch.End();
          var selectionRect = Editor.GetSubEntity<Selection>();
          if (Nez.Input.LeftMouseButtonPressed)
          {
            foreach (var shape in Gui.ShapeContext.Properties)
            {
              if (shape.Value is Shape propShape && propShape.Bounds.Contains(Entity.Scene.Camera.MouseToWorldPoint()))
              {
                selectionRect.Begin(propShape.Bounds, propShape);
                break;
              }
            } 
          }
          if (selectionRect.Capture is Shape theShape)
          {
            theShape.Bounds = selectionRect.Bounds;
          }
        }
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
        // Draw editor's tile names
        if (Editor.GetSubEntity<SheetView>().Enabled)
        {
          foreach (var tile in Editor.SpriteSheet.TileMap)
          {
            DrawText(tile.Value.Name, Editor.GetSubEntity<SheetView>().GetRegionInSheet(tile.Value.Region.ToRectangleF()).BottomLeft(), 
                2.5f/tile.Value.Name.Count());
          }   
        }
        else if (Editor.GetSubEntity<SpritexView>().Enabled && Gui.Selection is Sprites.Spritex spritex)
        {
          foreach (var part in spritex.Body)
          {
            DrawText(part.Name, part.Bounds.BottomCenter(), 1f/camera.RawZoom);
          }
        }
      }
    }
  }
}
