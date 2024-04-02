using Nez;
using Microsoft.Xna.Framework;
using ImGuiNET;
using Raven.Sheet.Sprites;

namespace Raven
{
  public class TilePainter
  {
    World _world;
    WorldEditor _worldEditor;
    List<Point> _canFillTiles = new List<Point>();
    public TilePainter(WorldEditor gui, Editor editor)
    {
      _worldEditor = gui;
      _world = gui._world;
    }
    public void HandleSelectedSprite()
    {
      var input = Core.GetGlobalManager<Input.InputManager>();
      var rawMouse = Nez.Input.RawMousePosition.ToVector2().ToNumerics();

      if (_worldEditor.SelectedSprite is Sprite sprite)
      {
        var min = sprite.Region.Location.ToVector2() / sprite.Texture.GetSize();
        var max = (sprite.Region.Location + sprite.Region.Size).ToVector2() / sprite.Texture.GetSize();
        var tilePos = rawMouse; 
        var tilesToPaint = sprite.GetRectTiles();

        void PaintAtLayer(TileLayer tileLayer, Point tileInLayer)
        {
          var tileStart = tilesToPaint.First();
          foreach (var tile in tilesToPaint)
          {
            var delta = tile.Coordinates - tileStart.Coordinates;
                 if (_worldEditor.PaintMode == PaintMode.Pen) tileLayer.ReplaceTile(tileInLayer + delta, new TileInstance(tile));
            else if (_worldEditor.PaintMode == PaintMode.Eraser) tileLayer.RemoveTile(tileInLayer + delta);
          }
        }
        void PaintPreviewAt(System.Numerics.Vector2 screenPos)
        { 

          ImGui.GetForegroundDrawList().AddImage(
              Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(sprite.Texture),
              screenPos - sprite.Region.GetHalfSize().ToNumerics() * _world.Scene.Camera.RawZoom, 
              screenPos - sprite.Region.GetHalfSize().ToNumerics() * _world.Scene.Camera.RawZoom + sprite.Region.Size.ToVector2().ToNumerics() 
              * _world.Scene.Camera.RawZoom,
              min.ToNumerics(), max.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());

        }

        // Show paint previews
        switch (_worldEditor.PaintType)
        {
          case PaintType.Single: PaintPreviewAt(tilePos); break;
          case PaintType.Rectangle:
            if (input.IsDrag && !input.IsDragFirst)
            {
              ImGui.GetForegroundDrawList().AddRectFilled(
                  input.MouseDragArea.Location.ToNumerics(), input.MouseDragArea.Max.ToNumerics(), 
                  _worldEditor.Editor.Settings.Colors.PickFill.ToColor().Add(new Color(0.5f, 0.5f, 0.6f, 0.1f)).ToImColor());
            } 
            break;
        }

        // Handle paint inputs; tile layer
        if (_world.CurrentLevel != null && _world.CurrentLevel.CurrentLayer is TileLayer tileLayer && !input.IsImGuiBlocking)
        {
          var spriteCenter = _world.Scene.Camera.MouseToWorldPoint() - sprite.Region.GetHalfSize() + sprite.TileSize.Divide(2, 2).ToVector2();

          // Single painting
          if (Nez.Input.LeftMouseButtonDown && _worldEditor.PaintType == PaintType.Single)
          {
            PaintAtLayer(tileLayer, tileLayer.GetTileCoordFromWorld(spriteCenter));
          }

          // Rectangle painting
          if (_worldEditor.PaintType == PaintType.Rectangle)
          {
            if (input.IsDragFirst) _mouseStart = _world.Scene.Camera.MouseToWorldPoint();
            if (input.IsDragLast) 
            {
              var rect = new RectangleF(); 
              rect.Location = _mouseStart;
              rect.Size = _world.Scene.Camera.MouseToWorldPoint() - _mouseStart;

              for (int x = 0; x < rect.Size.X; x+=sprite.Region.Size.X)
              {
                for (int y = 0; y < rect.Size.Y; y+=sprite.Region.Size.Y)
                {
                  PaintAtLayer(tileLayer, tileLayer.GetTileCoordFromWorld(rect.Location + new Vector2(x, y)));
                }
              }
            }
          }

          // Fill painting
          if (_worldEditor.PaintType == PaintType.Fill)
          {
            var tileInLayer = tileLayer.GetTileCoordFromWorld(spriteCenter); 
            var tileStart = tilesToPaint[tilesToPaint.Count/2];
            if (tileStart == null) return;
            _canFillTiles = _tileFiller.Update(tileLayer, tileInLayer, sprite.Region.Size/sprite.TileSize);
            if (Nez.Input.LeftMouseButtonPressed) 
            {
              void FloodFill(List<Point> fill)
              {
                foreach (var tile in _canFillTiles) 
                {
                  PaintAtLayer(tileLayer, tile);
                }
              }
              _tileFiller.Start(FloodFill);
            }
          }
        }
      }
      else if (_worldEditor.SelectedSprite is SpriteScene spriteScene)
      {
        foreach (var part in spriteScene.Parts)
        {
          var min = part.SourceSprite.Region.Location.ToVector2() / part.SourceSprite.Texture.GetSize();
          var max = (part.SourceSprite.Region.Location + part.SourceSprite.Region.Size).ToVector2() / part.SourceSprite.Texture.GetSize();
          var tilePos = rawMouse + part.LocalBounds.Location.ToNumerics();
          tilePos.X = (int)(tilePos.X / part.SourceSprite.TileSize.X) * part.SourceSprite.TileSize.X;
          tilePos.Y = (int)(tilePos.Y / part.SourceSprite.TileSize.Y) * part.SourceSprite.TileSize.Y; 

          ImGui.GetForegroundDrawList().AddImage(
              Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(part.SourceSprite.Texture),
              tilePos - spriteScene.EnclosingBounds.GetHalfSize().ToNumerics() * _world.Scene.Camera.RawZoom, 
              tilePos - spriteScene.EnclosingBounds.GetHalfSize().ToNumerics() + spriteScene.EnclosingBounds.Size.ToNumerics() * _world.Scene.Camera.RawZoom,
              min.ToNumerics(), max.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());

        }

        if (_world.CurrentLevel != null && _world.CurrentLevel.CurrentLayer is TileLayer tilelayer)
        {
          if (Nez.Input.LeftMouseButtonDown && !input.IsImGuiBlocking)
          { 
            var tileApprox = _world.Scene.Camera.MouseToWorldPoint() - spriteScene.Bounds.Size/2f; 
            var tileInLayer = tilelayer.GetTileCoordFromWorld(tileApprox); 
            tilelayer.ReplaceTile(tileInLayer, new SpriteSceneInstance(spriteScene));
          }
        }
        else if (_world.CurrentLevel != null && _world.CurrentLevel.CurrentLayer is FreeformLayer freeformLayer)
        {
          if (Nez.Input.LeftMouseButtonDown && !input.IsImGuiBlocking)
          { 

            var tileApprox = _world.Scene.Camera.MouseToWorldPoint() - spriteScene.Bounds.Size/2f; 
            var paint = freeformLayer.PaintSpriteScene(spriteScene);
            paint.Transform.LocalPosition = _world.Scene.Camera.MouseToWorldPoint();
          }
        }
      }
    }
    Vector2 _mouseStart = Vector2.Zero;
    TileFillFlooder _tileFiller = new TileFillFlooder();
  }
}
