using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;

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

        if (Gui.Selection is Sprites.Sprite sprite)
        {
          RenderLayer = Editor.WorldRenderLayer;
          batcher.DrawRect(Entity.Position+sprite.Region.Location.ToVector2(), 
              sprite.Region.Size.X * Entity.Scale.X, 
              sprite.Region.Size.Y * Entity.Scale.Y, 
              Editor.ColorSet.SpriteRegionActiveFill);
        }
        if (Editor.EditState == Editor.EditingState.Default) 
        {
          foreach (var tile in _tiles) 
          {
            var worldTile = tile.ToRectangleF();
            worldTile.Location += Parent._image.Bounds.Location;
            if (worldTile.Contains(camera.MouseToWorldPoint())) Parent._tileInMouse = tile;
            batcher.DrawRectOutline(camera, worldTile, Editor.ColorSet.SpriteRegionInactiveOutline);
          }
          // batcher.FlushBatch();
          // batcher.SetIgnoreRoundingDestinations(true);
          var worldTileInMouse = Parent._tileInMouse.ToRectangleF();
          if (worldTileInMouse != null) 
          {
            worldTileInMouse.Location += Parent._image.Bounds.Location;
            batcher.DrawRectOutline(camera, worldTileInMouse, Editor.ColorSet.SpriteRegionActiveFill);
          }
        }
        else 
        {
          batcher.DrawRect(Parent._image.Bounds, Editor.ColorSet.SpriteRegionInactiveOutline); 
          // Console.WriteLine($"pos: {pos}\n size: {size}\n camera: {camera.Position}\n camera zoom: {camera.RawZoom}\n Transform: {Entity.Transform.Position}");
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
      // else if (Collider) && Editor.EditState != Editor.EditingState.AutoRegion && !IsCollapsed)
      // {
      //   Editor.Set(Editor.EditingState.Default);
      // }

      if (IsCollapsed) return;
      if (Gui.Selection == null) Gui.SelectionRect.Enabled = false;
      if (Gui.SelectionRect != null && Gui.SelectionRect.IsEditingPoint) 
      {
        Gui.SelectionRect.Enabled = true;
        Gui.SelectionRect.Ren.Snap(Editor.TileWidth, Editor.TileHeight);
        Gui.SelectionRect.Update();
      }

      if (Editor.EditState == Editor.EditingState.Default )
      {
        SelectInput(); 
      }
    }
    void SelectInput()
    {
      if (Nez.Input.CurrentMouseState.LeftButton == ButtonState.Released || Nez.Input.CurrentMouseState.RightButton == ButtonState.Released)
      {
        if (Gui.Selection == null && IsSpritesView) 
        {
          foreach (var sprite in Editor.SpriteSheet.Sprites)
          {
            // if (ImUtils.HasMouseClickAt(sprite.Value.Region.ToRectangleF())) Select(sprite);
          }
        }
        else if (!IsSpritesView) 
        {
          for (int x = 0; x < Editor.SpriteSheet.Tiles.X; x++)
          {
            for (int y = 0; y < Editor.SpriteSheet.Tiles.Y; y++)
            {
              if (Editor.SpriteSheet.GetTile(x, y).Contains(Nez.Input.MousePosition))
              {
                Select(new Sheet.Tile(new Point(x,  y), Editor.SpriteSheet));
              }
            }
          }
        }
        if (!Gui.SelectionRect.IsEditingPoint && Gui.Selection != null) 
        {
          Gui.Selection = null;
          Editor.Set(Editor.EditingState.Default);
        }
      }
    }
    public void Select(object sel)
    {
      if (Gui.ShapeSelection != null) return;
      Gui.Selection = sel;
      if (Gui.Selection is Sprites.Sprite sprite)
      {
        Gui.SelectionRect = new Selection(Gui);
        Gui.SelectionRect.Ren.SetBounds(sprite.Region.ToRectangleF()); 
      }
      else if (Gui.Selection is Sheet.Tile)
      {
      }
      else if (Gui.Selection is Sprites.Spritex spritex)
      {
      }
      Editor.Set(Editor.EditingState.SelectedSprite);
    }
  }
}
