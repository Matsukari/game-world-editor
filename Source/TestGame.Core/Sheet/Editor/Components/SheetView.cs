using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using ImGuiNET;

namespace Raven.Sheet
{
  public class SheetView : Editor.SubEntity
  {
    public bool IsCollapsed = false;
    public bool IsSpritesView = false;
    internal Rectangle TileInMouse;
    SpriteRenderer _image;
    public override void OnAddedToScene()
    {
      _image = AddComponent(new SpriteRenderer(Gui.SheetTexture));
      AddComponent(new Renderable());
      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent());
    }    
    public class Renderable : Editor.SubEntity.RenderableComponent<SheetView>
    {
      List<Rectangle> _tiles;
      public override void OnAddedToEntity()
      {
        _tiles = Editor.SpriteSheet.GetTiles();
      }      
      public override void Render(Batcher batcher, Camera camera)
      {
        if (Editor.SpriteSheet == null) return;
        DrawArtifacts(batcher, camera);

        // Draw last selected sprite
        RectangleF region = RectangleF.Empty;
        if (Gui.Selection is Sprites.Sprite sprite) region = sprite.Region.ToRectangleF();
        else if (Gui.Selection is Sprites.Tile tile) region = tile.Region.ToRectangleF();
        if (region != RectangleF.Empty) batcher.DrawRect(Parent.GetRegionInSheet(region), Editor.ColorSet.SpriteRegionActiveFill);
      }
      // Rectangles & highlights
      void DrawArtifacts(Batcher batcher, Camera camera)
      {
        if (Editor.EditState == Editor.EditingState.Default) 
        {
          // Draw tiles' grid
          foreach (var tile in _tiles) 
          {
            var worldTile = tile.ToRectangleF();
            worldTile.Location += Parent._image.Bounds.Location;
            if (worldTile.Contains(camera.MouseToWorldPoint())) Parent.TileInMouse = tile;
            batcher.DrawRectOutline(camera, worldTile, Editor.ColorSet.SpriteRegionInactiveOutline);
          }
          // Highlight the tile under mouse
          if (!Parent.IsSpritesView)
          {
            var worldTileInMouse = Parent.TileInMouse.ToRectangleF();
            if (worldTileInMouse != null) 
            {
              worldTileInMouse.Location += Parent._image.Bounds.Location;
              batcher.DrawRectOutline(camera, worldTileInMouse, Editor.ColorSet.SpriteRegionActiveFill);
            }
          }
        }
        // Darken the whole image when under different editing state
        else 
        {
          batcher.DrawRect(Parent._image.Bounds, Editor.ColorSet.SpriteRegionInactiveOutline); 
        }
      }
    }
        
    public override void Update()
    {
      base.Update();
      if (Nez.Input.IsKeyReleased(Keys.Escape) && IsSpritesView)
      {
        Editor.GetSubEntity<SpritexView>().UnEdit();
      }
      else if (_image.Bounds.Contains(Scene.Camera.MouseToWorldPoint()) && HasNoObstruction())
      {
        Editor.Set(Editor.EditingState.Default);
      }

      if (IsSpritesView) return;
      if (Gui.Selection == null) Gui.SelectionRect.Enabled = false;
      if (Gui.SelectionRect != null && Gui.SelectionRect.IsEditingPoint) 
      {
        Gui.SelectionRect.Enabled = true;
        Gui.SelectionRect.Ren.Snap(Editor.TileWidth, Editor.TileHeight);
        Gui.SelectionRect.Update();
      }
    }
    public RectangleF GetRegionInSheet(RectangleF rectangle)
    {
      rectangle.Location += _image.Bounds.Location;
      return rectangle;
    }
    internal bool HasNoObstruction()
    {
      return !IsCollapsed && !ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) && !ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow)
        && Editor.EditState != Editor.EditingState.AnnotateShape;
    }
  }
}
