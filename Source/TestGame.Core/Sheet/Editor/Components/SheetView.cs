using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using ImGuiNET;

namespace Raven.Sheet
{
  public class SheetView : Editor.SheetEntity
  {
    internal Rectangle TileInMouse;
    SpriteRenderer _image;
    public override void OnChangedTab()
    {
      Position = Vector2.Zero;
      RemoveAllComponents(); 
      _image = AddComponent(new SpriteRenderer(Sheet.Texture));
      AddComponent(new Renderable());
      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent());
    }    
    public class Renderable : Editor.SheetEntity.Renderable<SheetView>
    {
      List<Rectangle> _tiles = new List<Rectangle>();
      Point lastSize = Point.Zero;
      public override void Render(Batcher batcher, Camera camera)
      {
        SyncModifiedTiles();
        DrawArtifacts(batcher, camera);

        // Draw last selected sprite
        RectangleF region = RectangleF.Empty;
        if (Gui.Selection is Sprites.Sprite sprite) region = sprite.Region.ToRectangleF();
        else if (Gui.Selection is Sprites.Tile tile) region = tile.Region.ToRectangleF();
        if (region != RectangleF.Empty) batcher.DrawRect(Parent.GetRegionInSheet(region), Editor.ColorSet.SpriteRegionActiveFill);
      }
      void SyncModifiedTiles()
      {
        if (lastSize != Sheet.TileSize)
        {
          lastSize = Sheet.TileSize;
          _tiles = Sheet.GetTiles();
        }
      }
      // Rectangles & highlights
      void DrawArtifacts(Batcher batcher, Camera camera)
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
        var worldTileInMouse = Parent.TileInMouse.ToRectangleF();
        if (worldTileInMouse != null) 
        {
          worldTileInMouse.Location += Parent._image.Bounds.Location;
          batcher.DrawRectOutline(camera, worldTileInMouse, Editor.ColorSet.SpriteRegionActiveFill);
        }
      }
    }
        
    public override void Update()
    {
      base.Update();
      if (_image.Bounds.Contains(Scene.Camera.MouseToWorldPoint()) && HasNoObstruction())
      {
        Editor.Set(Editor.EditingState.Default);
      }
    }
    public RectangleF GetRegionInSheet(RectangleF rectangle)
    {
      rectangle.Location += _image.Bounds.Location;
      return rectangle;
    }
    public Vector2 GetOffRegionInSheet(Vector2 vector)
    {
      vector -= _image.Bounds.Location;
      return vector;
    }
    internal bool HasNoObstruction()
    {
      return 
        Enabled  
        && !ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) 
        && !ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow)
        && Editor.EditState == Editor.EditingState.Default;
    }
  }
}
