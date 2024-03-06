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
    Rectangle _tileInMouse;
    SpriteRenderer _image;
    public override void OnAddedToScene()
    {
      _image = AddComponent(new SpriteRenderer(Gui.SheetTexture));
      // _image.Origin = new Vector2();
      // LocalScale = Screen.Size / Gui.SheetTexture.GetSize();
      // var min = Math.Min(LocalScale.X, LocalScale.Y);
      // LocalScale = new Vector2(min, min);
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
        RectangleF region = RectangleF.Empty;
        if (Gui.Selection is Sprites.Sprite sprite) region = sprite.Region.ToRectangleF();
        else if (Gui.Selection is Sprites.Tile tile) region = tile.Region.ToRectangleF();
        if (region != RectangleF.Empty) batcher.DrawRect(Parent.GetRegionInSheet(region), Editor.ColorSet.SpriteRegionActiveFill);
      }
      void DrawArtifacts(Batcher batcher, Camera camera)
      {
        if (Editor.EditState == Editor.EditingState.Default) 
        {
          foreach (var tile in _tiles) 
          {
            var worldTile = tile.ToRectangleF();
            worldTile.Location += Parent._image.Bounds.Location;
            if (worldTile.Contains(camera.MouseToWorldPoint())) Parent._tileInMouse = tile;
            batcher.DrawRectOutline(camera, worldTile, Editor.ColorSet.SpriteRegionInactiveOutline);
          }
          if (!Parent.IsSpritesView)
          {
            var worldTileInMouse = Parent._tileInMouse.ToRectangleF();
            if (worldTileInMouse != null) 
            {
              worldTileInMouse.Location += Parent._image.Bounds.Location;
              batcher.DrawRectOutline(camera, worldTileInMouse, Editor.ColorSet.SpriteRegionActiveFill);
            }
          }
        }
        else 
        {
          batcher.DrawRect(Parent._image.Bounds, Editor.ColorSet.SpriteRegionInactiveOutline); 
        }
      }
    }
        
    public override void Update()
    {
      base.Update();
      if (Nez.Input.IsKeyReleased(Keys.Escape) && IsCollapsed)
      {
        Editor.GetSubEntity<SpritexView>().UnEdit();
      }
      else if (_image.Bounds.Contains(Scene.Camera.MouseToWorldPoint()) && HasNoObstruction())
      {
        Editor.Set(Editor.EditingState.Default);
      }

      if (IsCollapsed) return;
      if (Gui.Selection == null) Gui.SelectionRect.Enabled = false;
      if (Gui.SelectionRect != null && Gui.SelectionRect.IsEditingPoint) 
      {
        Gui.SelectionRect.Enabled = true;
        Gui.SelectionRect.Ren.Snap(Editor.TileWidth, Editor.TileHeight);
        Gui.SelectionRect.Update();
      }

      if (Editor.EditState == Editor.EditingState.Default && HasNoObstruction())
      {
        SelectInput(); 
      }
    }
    void SelectInput()
    {
      if (Nez.Input.LeftMouseButtonReleased || Nez.Input.RightMouseButtonReleased)
      {
        if (IsSpritesView) 
        {
          foreach (var sprite in Editor.SpriteSheet.Sprites)
          {
            if (GetRegionInSheet(sprite.Value.Region.ToRectangleF()).Contains(Scene.Camera.MouseToWorldPoint())) 
              Select(sprite.Value);
          }
        }
        else if (!IsSpritesView && _tileInMouse != null) 
        {
          var coord = Editor.SpriteSheet.GetTile(_tileInMouse);
          var tileInCoord = Editor.SpriteSheet.CustomTileExists(coord.X, coord.Y);
          if (tileInCoord != null) Select(tileInCoord);
          else Select(new Sprites.Tile(coord, Editor.SpriteSheet));
        }
        else if (!Gui.SelectionRect.IsEditingPoint && Gui.Selection != null) 
        {
          Gui.Selection = null;
          Editor.Set(Editor.EditingState.Default);
        }
      }
    }
    public void Select(IPropertied sel)
    {
      if (Gui.ShapeSelection != null) return;
      Gui.Selection = sel;
      if (Gui.Selection is Sprites.Sprite sprite)
      {
        Gui.SelectionRect = new Selection(Gui);
        Gui.SelectionRect.Ren.SetBounds(sprite.Region.ToRectangleF()); 
      }
      else if (Gui.Selection is Sprites.Tile tile)
      {
      }
      else if (Gui.Selection is Sprites.Spritex spritex)
      {
      }
      else throw new TypeAccessException();
      Console.WriteLine($"Selected {Gui.Selection.GetType().Name}");
      // Editor.Set(Editor.EditingState.SelectedSprite);
    }
    RectangleF GetRegionInSheet(RectangleF rectangle)
    {
      rectangle.Location += _image.Bounds.Location;
      return rectangle;
    }
    bool HasNoObstruction()
    {
      return !IsCollapsed && !ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) && !ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow);
    }
  }
}
