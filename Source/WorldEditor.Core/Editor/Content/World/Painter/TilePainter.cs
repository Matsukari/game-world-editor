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
    public event Action<FreeformLayer, SpriteSceneInstance> OnPaintScene;

    public TilePainter(WorldView view)
    {
      _view = view;
      _world = view.Content as World;
    }
    bool IInputHandler.OnHandleInput(Raven.InputManager input)
    {
      var level = _view.Window.SelectedLevelInspector; 
      if (level == null) return false;

      var layer = level.CurrentLayer; 
      if (layer == null) return false;

      // Handles spriteScene
      if (Nez.Input.LeftMouseButtonPressed && layer is FreeformLayer freeformLayer)
      {
        if (_view.SpritePicker.SelectedSprite is SpriteScene spriteScene)
        {
          if (_view.PaintType == PaintType.Single)
            PaintSpriteScene(freeformLayer, spriteScene);
        }
        else if (_view.PaintMode == PaintMode.Eraser)
          PaintSpriteScene(freeformLayer, null);

        return true;
      }

      // Handles tile and sprites
      else if (layer is TileLayer tileLayer)
      {
        if (_view.SpritePicker.SelectedSprite is Sprite sprite)
        {
          var spriteCenter = InputManager.GetWorldMousePosition(Camera) - sprite.Region.GetHalfSize() + sprite.TileSize.Divide(2, 2).ToVector2();
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
            if (input.IsDragFirst && Nez.Input.LeftMouseButtonDown) _mouseStart = InputManager.GetWorldMousePosition(Camera);
            if (input.IsDragLast && Nez.Input.LeftMouseButtonReleased) 
            {
              var rect = new RectangleF(); 
              rect.Location = _mouseStart;
              rect.Size = InputManager.GetWorldMousePosition(Camera) - _mouseStart;
              rect = rect.AlwaysPositive();

              if (_view.PaintMode == PaintMode.Eraser)
              {
                for (int x = 0; x < rect.Size.X; x+=tileLayer.TileWidth)
                {
                  for (int y = 0; y < rect.Size.Y; y+=tileLayer.TileHeight)
                  {
                    tileLayer.RemoveTile(tileLayer.GetTileCoordFromWorld(rect.Location + new Vector2(x, y)));
                  }
                }
              }
              else 
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
      }
      return false;
    }
    void PaintAtLayer(List<Tile> tilesToPaint, TileLayer tileLayer, Point tileInLayer)
    {
      if (tilesToPaint.Count() == 0) return;
      var tileStart = tilesToPaint.First();
      for (int i = 0; i < tilesToPaint.Count(); i++)
      {
        var delta = tilesToPaint[i].Coordinates - tileStart.Coordinates;
        var tile = (_view.IsRandomPaint) ? tilesToPaint.RandomItem() : tilesToPaint[i];
        if (_view.PaintMode == PaintMode.Pen) tileLayer.ReplaceTile(tileInLayer + delta, tile);
        else if (_view.PaintMode == PaintMode.Eraser) tileLayer.RemoveTile(tileInLayer + delta);
      }
    }
    void PaintSpriteScene(FreeformLayer freeformLayer, SpriteScene scene)
    {
      var pos = InputManager.GetWorldMousePosition(Camera); 
      if (_view.PaintMode == PaintMode.Pen)
      {
        var paint = freeformLayer.PaintSpriteScene(scene);
        paint.Transform.Position = pos - freeformLayer.Bounds.Location;
      }
      else if (_view.PaintMode == PaintMode.Eraser)
      {
        freeformLayer.RemoveSpriteSceneAt(InputManager.GetWorldMousePosition(Camera));
      }
    }
    public override void OnHandleSelectedSprite()
    {
      var input = Core.GetGlobalManager<InputManager>();
      var rawMouse = InputManager.ScreenMousePosition.ToNumerics();

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
            if (Nez.Input.LeftMouseButtonDown)
            {
              if (_view.PaintMode == PaintMode.Pen)
              {
                var dragArea = input.MouseDragArea.AlwaysPositive();
                for (float x = 0; x < dragArea.Size.X; x+=sprite.Region.Size.X * Camera.RawZoom)
                {
                  for (float y = 0; y < dragArea.Size.Y; y+=sprite.Region.Size.Y * Camera.RawZoom)
                  {
                    ImGui.GetForegroundDrawList().AddImage(
                        Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(sprite.Texture),
                        (dragArea.Location + new Vector2(x, y)).ToNumerics(), 
                        (dragArea.Location + new Vector2(x, y)).ToNumerics() + sprite.Region.Size.ToVector2().ToNumerics() * Camera.RawZoom,
                        min.ToNumerics(), max.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());
                  }
                }
              }
              else
              {
                ImGui.GetBackgroundDrawList().AddRectFilled(
                    input.MouseDragArea.Location.ToNumerics(), input.MouseDragArea.Max.ToNumerics(), 
                    _view.Settings.Colors.PickFill.ToColor().ToImColor());
                ImGui.GetBackgroundDrawList().AddRect(
                    input.MouseDragArea.Location.ToNumerics(), input.MouseDragArea.Max.ToNumerics(), 
                    _view.Settings.Colors.PickHoverOutline.ToColor().ToImColor());
              } 
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
          var pos = rawMouse + part.Bounds.Location.ToNumerics() * Camera.RawZoom;

          ImGui.GetForegroundDrawList().AddImage(
              Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(part.SourceSprite.Texture),
              pos, 
              pos + part.Bounds.Size.ToNumerics() * Camera.RawZoom,
              min.ToNumerics(), max.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());
        }
      }
    }
    Vector2 _mouseStart = Vector2.Zero;
    TileFillFlooder _tileFiller = new TileFillFlooder();
  }
}