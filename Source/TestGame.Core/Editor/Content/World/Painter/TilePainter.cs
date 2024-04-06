using Nez;
using Microsoft.Xna.Framework;
using ImGuiNET;

namespace Raven
{
  public class TilePainter : SpritePicker, IInputHandler
  {
    World _world;
    WorldView _view;
    List<Point> _canFillTiles = new List<Point>();
    Camera Camera { get => _view.Camera; }

    public TilePainter(WorldView view)
    {
      _view = view;
      _world = view.WorldEntity.World;
    }
    bool IInputHandler.OnHandleInput(Raven.InputManager input)
    {
      // Handles spriteScene
      if (Nez.Input.LeftMouseButtonDown 
          && _view.Window.SelectedLevelInspector != null 
          && _view.Window.SelectedLevelInspector.CurrentLayer is FreeformLayer freeformLayer 
          && _view.SpritePicker.SelectedSprite is SpriteScene spriteScene)
      {
        PaintSpriteScene(freeformLayer, spriteScene);
        return true;
      }

      // Handles tile and sprites
      else if (
             _view.Window.SelectedLevelInspector != null 
          && _view.Window.SelectedLevelInspector.CurrentLayer is TileLayer tileLayer
          && _view.SpritePicker.SelectedSprite is Sprite sprite)
        {
          var spriteCenter = Camera.MouseToWorldPoint() - sprite.Region.GetHalfSize() + sprite.TileSize.Divide(2, 2).ToVector2();
          var tilesToPaint = sprite.GetRectTiles();

          // Single painting
          if (Nez.Input.LeftMouseButtonDown && _view.PaintType == PaintType.Single)
          {
            PaintAtLayer(tilesToPaint, tileLayer, tileLayer.GetTileCoordFromWorld(spriteCenter));
            return true;
          }

          // Rectangle painting
          if (_view.PaintType == PaintType.Rectangle)
          {
            if (input.IsDragFirst) _mouseStart = Camera.MouseToWorldPoint();
            if (input.IsDragLast) 
            {
              var rect = new RectangleF(); 
              rect.Location = _mouseStart;
              rect.Size = Camera.MouseToWorldPoint() - _mouseStart;

              for (int x = 0; x < rect.Size.X; x+=sprite.Region.Size.X)
              {
                for (int y = 0; y < rect.Size.Y; y+=sprite.Region.Size.Y)
                {
                  PaintAtLayer(tilesToPaint, tileLayer, tileLayer.GetTileCoordFromWorld(rect.Location + new Vector2(x, y)));
                }
              }
            }
            return input.IsDrag;
          }

          // Fill painting
          if (_view.PaintType == PaintType.Fill)
          {
            var tileInLayer = tileLayer.GetTileCoordFromWorld(spriteCenter); 
            var tileStart = tilesToPaint[tilesToPaint.Count/2];
            if (tileStart == null) return false;
            _canFillTiles = _tileFiller.Update(tileLayer, tileInLayer, sprite.Region.Size/sprite.TileSize);
            if (Nez.Input.LeftMouseButtonPressed) 
            {
              void FloodFill(List<Point> fill)
              {
                foreach (var tile in _canFillTiles) 
                {
                  PaintAtLayer(tilesToPaint, tileLayer, tile);
                }
              }
              _tileFiller.Start(FloodFill);
              return true;
            }
          }
        }
      return false;
    }
    void PaintAtLayer(List<Tile> tilesToPaint, TileLayer tileLayer, Point tileInLayer)
    {
      var tileStart = tilesToPaint.First();
      foreach (var tile in tilesToPaint)
      {
        var delta = tile.Coordinates - tileStart.Coordinates;
        if (_view.PaintMode == PaintMode.Pen) tileLayer.ReplaceTile(tileInLayer + delta, tile);
        else if (_view.PaintMode == PaintMode.Eraser) tileLayer.RemoveTile(tileInLayer + delta);
      }
    }
    void PaintSpriteScene(FreeformLayer freeformLayer, SpriteScene scene)
    {
      var tileApprox = Camera.MouseToWorldPoint() - scene.EnclosingBounds.Size/2f; 
      var paint = freeformLayer.PaintSpriteScene(scene);
      paint.Transform.Position = Camera.MouseToWorldPoint();
    }
    void PaintTile(Point coord)
    {

    }
    public override void OnHandleSelectedSprite()
    {
      var input = Core.GetGlobalManager<InputManager>();
      var rawMouse = Nez.Input.RawMousePosition.ToVector2().ToNumerics();

      if (_view.SpritePicker.SelectedSprite is Sprite sprite)
      {
        var min = sprite.Region.Location.ToVector2() / sprite.Texture.GetSize();
        var max = (sprite.Region.Location + sprite.Region.Size).ToVector2() / sprite.Texture.GetSize();
        var tilePos = rawMouse; 
        void PaintPreviewAt(System.Numerics.Vector2 screenPos)
        { 

          ImGui.GetForegroundDrawList().AddImage(
              Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(sprite.Texture),
              screenPos - sprite.Region.GetHalfSize().ToNumerics() * Camera.RawZoom, 
              screenPos - sprite.Region.GetHalfSize().ToNumerics() * Camera.RawZoom + sprite.Region.Size.ToVector2().ToNumerics() * Camera.RawZoom,
              min.ToNumerics(), max.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());

        }

        // Show paint previews
        switch (_view.PaintType)
        {
          case PaintType.Single: PaintPreviewAt(tilePos); break;
          case PaintType.Rectangle:
            if (input.IsDrag && !input.IsDragFirst)
            {
              ImGui.GetForegroundDrawList().AddRectFilled(
                  input.MouseDragArea.Location.ToNumerics(), input.MouseDragArea.Max.ToNumerics(), 
                  _view.Settings.Colors.PickFill.ToColor().Add(new Color(0.5f, 0.5f, 0.6f, 0.1f)).ToImColor());
            } 
            break;
        }


      }
      else if (_view.SpritePicker.SelectedSprite is SpriteScene spriteScene)
      {
        foreach (var part in spriteScene.Parts)
        {
          var min = part.SourceSprite.Region.Location.ToVector2() / part.SourceSprite.Texture.GetSize();
          var max = (part.SourceSprite.Region.Location + part.SourceSprite.Region.Size).ToVector2() / part.SourceSprite.Texture.GetSize();
          var tilePos = rawMouse + part.Bounds.Location.ToNumerics();
          tilePos.X = (int)(tilePos.X / part.SourceSprite.TileSize.X) * part.SourceSprite.TileSize.X;
          tilePos.Y = (int)(tilePos.Y / part.SourceSprite.TileSize.Y) * part.SourceSprite.TileSize.Y; 

          ImGui.GetForegroundDrawList().AddImage(
              Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(part.SourceSprite.Texture),
              tilePos - spriteScene.EnclosingBounds.GetHalfSize().ToNumerics() * Camera.RawZoom, 
              tilePos - spriteScene.EnclosingBounds.GetHalfSize().ToNumerics() + spriteScene.EnclosingBounds.Size.ToNumerics() * Camera.RawZoom,
              min.ToNumerics(), max.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());
        }
      }
    }
    Vector2 _mouseStart = Vector2.Zero;
    TileFillFlooder _tileFiller = new TileFillFlooder();
  }
}
