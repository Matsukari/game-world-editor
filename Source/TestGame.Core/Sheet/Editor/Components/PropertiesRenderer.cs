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
      AddComponent(new Renderable());
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
        if (Editor.GetSubEntity<SheetView>().Enabled)
        {
          Gui.primitiveBatch.Begin(camera.ProjectionMatrix, camera.TransformMatrix);
          Annotator.Renderable.DrawPropertiesShapes(Editor.SpriteSheet, Gui.primitiveBatch, batcher, camera, Editor.ColorSet.AnnotatedShapeInactive);
          Gui.primitiveBatch.End();
          foreach (var tile in Editor.SpriteSheet.TileMap)
          {
            batcher.DrawString(
                Graphics.Instance.BitmapFont, 
                tile.Value.Name, 
                Editor.GetSubEntity<SheetView>().GetRegionInSheet(tile.Value.Region.ToRectangleF()).Location, 
                color: Color.White, 
                rotation: 0f, 
                origin: Editor.SpriteSheet.TileSize.ToVector2(), 
                scale: 1, 
                effects: Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 
                layerDepth: 0f);
          }
        }
      }
    }
  }
}
