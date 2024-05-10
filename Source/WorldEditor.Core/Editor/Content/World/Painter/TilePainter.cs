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
    Layer _currentLayer;

    public TilePainter(WorldView view)
    {
      _view = view;
      _world = view.Content as World;
    }
    bool IInputHandler.OnHandleInput(Raven.InputManager input)
    {
      var layer = _view.Window.CurrentLayer;
      if (layer == null) return false;
      _currentLayer = layer;

      if (_view.PaintMode == PaintMode.Inspector && layer is TileLayer tileLayer1)
      {
        var mouse = InputManager.GetWorldMousePosition(input.Camera);
        var tileCoord = tileLayer1.GetTileCoordFromWorld(mouse); 
        var tile = tileLayer1.TryGetTile(tileCoord);
        if (tile != null && Nez.Input.LeftMouseButtonReleased && OnSelectTileInstance != null) 
        {
          OnSelectTileInstance(tile, tileLayer1);
          return true;
        }
      }

      if (_view.PaintMode == PaintMode.None || _view.PaintMode == PaintMode.Inspector || layer.IsLocked || !layer.IsVisible) return false;

      if (Nez.Input.LeftMouseButtonPressed) 
        _mouseStart = InputManager.GetWorldMousePosition(Camera);

      if (Nez.Input.LeftMouseButtonReleased) 
      {
        RecordTiles();
      }

      var mouseDrag = new RectangleF(_mouseStart, InputManager.GetWorldMousePosition(Camera) - _mouseStart).AlwaysPositive();


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
        if (_view.SpritePicker.SelectedSprite is Sprite sprite && _view.PaintMode == PaintMode.Pen)
        {
          var spriteCenter = InputManager.GetWorldMousePosition(Camera) - sprite.Region.GetHalfSize() + sprite.TileSize.Divide(2, 2).ToVector2();
          var tilesToPaint = sprite.GetRectTiles();

          // Single painting
          if (Nez.Input.LeftMouseButtonDown && _view.PaintType == PaintType.Single)
          {
            PaintAtLayer(tilesToPaint, tileLayer, tileLayer.GetTileCoordFromWorld(spriteCenter));
            return true;
          }
          else if (Nez.Input.LeftMouseButtonReleased && _view.PaintType == PaintType.Rectangle)
          {
            Rectangle(tileLayer, mouseDrag, sprite.Region.Size, (coord)=>PaintAtLayer(tilesToPaint, tileLayer, coord));
            return true;
          }
          else if (_view.PaintType == PaintType.Fill && Fill(tileLayer, sprite, tilesToPaint)) 
            return true;
        }
        else if (_view.PaintMode == PaintMode.Eraser)
        {
          if (Nez.Input.LeftMouseButtonDown && _view.PaintType == PaintType.Single)
          {
            RemoveAtLayer(tileLayer, tileLayer.GetTileCoordFromWorld(InputManager.GetWorldMousePosition(Camera)));
            return true;
          }
          else if (Nez.Input.LeftMouseButtonReleased && _view.PaintType == PaintType.Rectangle)
          {
            Rectangle(tileLayer, mouseDrag, tileLayer.TileSize, (coord)=>RemoveAtLayer(tileLayer, coord));
            return true;
          }
          else if (_view.PaintType == PaintType.Fill && Fill(tileLayer, null, null)) 
            return true;      
        }
      }
      return false;
    }
    void Rectangle(TileLayer tileLayer, RectangleF rect, Point step, Action<Point> callback)
    {
      for (int x = 0; x < rect.Size.X; x+=step.X)
      {
        for (int y = 0; y < rect.Size.Y; y+=step.Y)
        {
          callback.Invoke(tileLayer.GetTileCoordFromWorld(rect.Location + new Vector2(x, y)));
        }
      }
      RecordTiles();
    }
    bool Fill(TileLayer tileLayer, Sprite sprite, List<Tile> tilesToPaint)
    {
      Point tileInLayer = tileLayer.GetTileCoordFromWorld(InputManager.GetWorldMousePosition(Camera));
      Point agentSize = new Point(1, 1);
      if (sprite != null)
      {
        var spriteCenter = InputManager.GetWorldMousePosition(Camera) - sprite.Region.GetHalfSize() + sprite.TileSize.Divide(2, 2).ToVector2();
        tileInLayer = tileLayer.GetTileCoordFromWorld(spriteCenter); 
        agentSize = sprite.Region.Size/sprite.TileSize;
        var tileStart = tilesToPaint[tilesToPaint.Count/2];
        if (tileStart == null) return false;
      }
      if (Nez.Input.LeftMouseButtonReleased)
      {
        void FloodFill(List<Point> fill)
        {
          foreach (var tile in _canFillTiles) 
          {
            if (_view.PaintMode == PaintMode.Pen) PaintAtLayer(tilesToPaint, tileLayer, tile);
            else if (_view.PaintMode == PaintMode.Eraser) RemoveAtLayer(tileLayer, tile);
          }
          RecordTiles();
        }

        _tileFiller.Start(FloodFill);
      }
      _canFillTiles = _tileFiller.Update(tileLayer, tileInLayer, agentSize);
      return false;
    }
    Dictionary<Point, Command> _commandGroup = new Dictionary<Point, Command>();

    void RecordTiles()
    {
      if (_commandGroup.Count() == 0) return;
      Core.GetGlobalManager<CommandManagerHead>().Current.Record(new CommandGroup(_commandGroup.Values.ToList()));
      _commandGroup.Clear();
    }

    void RemoveAtLayer(TileLayer tileLayer, Point tileInLayer)
    {
      var previous = tileLayer.TryGetTile(tileInLayer);
      tileLayer.RemoveTile(tileInLayer);
      if (_commandGroup.ContainsKey(tileInLayer) && _commandGroup[tileInLayer] is RemovePaintTileCommand paint)
      {
        previous = paint._last;
        // Console.WriteLine($"Delegated to{previous == null}");
      }
      _commandGroup[tileInLayer] = new RemovePaintTileCommand(tileLayer, previous, tileInLayer);
    }
    void PaintAtLayer(List<Tile> tilesToPaint, TileLayer tileLayer, Point tileInLayer)
    {
      if (tilesToPaint.Count() == 0) return;
      var tileStart = tilesToPaint.First();
      for (int i = 0; i < tilesToPaint.Count(); i++)
      {
        var delta = tilesToPaint[i].Coordinates - tileStart.Coordinates;
        var tile = (_view.IsRandomPaint) ? tilesToPaint.RandomItem() : tilesToPaint[i];

        var coord = tileInLayer + delta;
        var previous = tileLayer.TryGetTile(coord);
        tileLayer.ReplaceTile(coord, tile);
        if (_commandGroup.ContainsKey(coord) && _commandGroup[coord] is PaintTileCommand paint)
        {
          previous = paint._start;
          // Console.WriteLine($"Delegated to{previous == null}");
        }
        // Console.WriteLine($"start is null? {previous == null}");
        _commandGroup[coord] = new PaintTileCommand(tileLayer, tileLayer.TryGetTile(coord), previous, coord);

      }
    }
    void PaintSpriteScene(FreeformLayer freeformLayer, SpriteScene scene)
    {
      var pos = InputManager.GetWorldMousePosition(Camera); 
      if (_view.PaintMode == PaintMode.Pen)
      {
        var paint = freeformLayer.PaintSpriteScene(scene);
        var previous = paint.Props.Transform.Copy();
        paint.Props.Transform.Position = pos - freeformLayer.Bounds.Location;

        Core.GetGlobalManager<CommandManagerHead>().Current.Record(
            new CommandGroup(new PaintSceneCommand(freeformLayer, paint), 
            new RenderPropTransformModifyCommand(paint.Props, previous)));
      }
      else if (_view.PaintMode == PaintMode.Eraser)
      {
        var instance = freeformLayer.RemoveSpriteSceneAt(InputManager.GetWorldMousePosition(Camera));
        if (instance != null)
          Core.GetGlobalManager<CommandManagerHead>().Current.Record(new RemoveSceneCommand(freeformLayer, instance));

      }
    }
    public override void OnHandleSelectedSpriteInside()
    {
      if (_view.PaintMode == PaintMode.Pen && _view.PaintType == PaintType.Single && SelectedSprite is Sprite sprite && _currentLayer is TileLayer)
      {
        var pos = InputManager.ScreenMousePosition.ToNumerics();
        ImGui.GetForegroundDrawList().AddImage(
            Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(sprite.Texture),
            pos - sprite.Region.GetHalfSize().ToNumerics() * Camera.RawZoom, 
            pos - sprite.Region.GetHalfSize().ToNumerics() * Camera.RawZoom + sprite.Region.Size.ToVector2().ToNumerics() * Camera.RawZoom,
            sprite.MinUv.ToNumerics(), sprite.MaxUv.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor()); 
      }
    }

    public override void OnHandleSelectedSprite()
    {
      var input = Core.GetGlobalManager<InputManager>();
      var rawMouse = InputManager.ScreenMousePosition.ToNumerics();

      if (_view.PaintMode != PaintMode.Pen) return;

      if (SelectedSprite is Sprite sprite && _currentLayer is TileLayer tileLayer)
      {
        void PaintPreviewAt(System.Numerics.Vector2 screenPos)
        { 
          ImGui.GetBackgroundDrawList().AddImage(
              Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(sprite.Texture),
              screenPos - sprite.Region.GetHalfSize().ToNumerics() * Camera.RawZoom, 
              screenPos - sprite.Region.GetHalfSize().ToNumerics() * Camera.RawZoom + sprite.Region.Size.ToVector2().ToNumerics() * Camera.RawZoom,
              sprite.MinUv.ToNumerics(), sprite.MaxUv.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());
        }

        // Show paint previews
        switch (_view.PaintType)
        {
          case PaintType.Single: PaintPreviewAt(Camera.WorldToScreenPoint(PosToTiled(InputManager.GetWorldMousePosition(Camera))).ToNumerics()); break;
          case PaintType.Rectangle:
            if (Nez.Input.LeftMouseButtonPressed) _mouseStartPaint = InputManager.GetWorldMousePosition(Camera);
            if (Nez.Input.LeftMouseButtonDown)
            {
              var dragArea = new RectangleF(_mouseStartPaint, InputManager.GetWorldMousePosition(Camera) - _mouseStartPaint);

              var absoluteArea = dragArea.AlwaysPositive();
              absoluteArea.Location = PosToTiled(absoluteArea.Location);
              // absoluteArea.Size /= Camera.RawZoom;
              absoluteArea.Size = absoluteArea.Size.MathMax(sprite.Region.Size.ToVector2());

              for (float x = 0; x < absoluteArea.Size.X; x+=sprite.Region.Size.X)
              {
                for (float y = 0; y < absoluteArea.Size.Y; y+=sprite.Region.Size.Y)
                {
                  var delta = new Vector2(x, y);
                  // if ((InputManager.GetWorldMousePosition(Camera) - _mouseStartPaint).EitherIsNegative()) delta = delta.Negate();

                  var bounds = RectangleF.FromMinMax(
                      absoluteArea.Location + delta, 
                      absoluteArea.Location + delta + sprite.Region.Size.ToVector2()).AlwaysPositive();

                  ImGui.GetBackgroundDrawList().AddImage(
                      Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(sprite.Texture),
                      Camera.WorldToScreenPoint(bounds.Location).ToNumerics(), 
                      Camera.WorldToScreenPoint(bounds.Max).ToNumerics(),
                      sprite.MinUv.ToNumerics(), sprite.MaxUv.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());
                }
              }
            }
            break;
        }
      }
      else if (SelectedSprite is SpriteScene spriteScene && _currentLayer is FreeformLayer)
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
      else if (SelectedSprite is SpriteScene && _currentLayer is TileLayer)
      {
        ImGui.BeginTooltip();
        ImGui.TextDisabled("Please select a tile to paint on this layer.");
        ImGui.EndTooltip();
      }
      else if (_currentLayer is FreeformLayer && _view.PaintMode == PaintMode.Pen && SelectedSprite is not SpriteScene)
      {
        ImGui.BeginTooltip();
        ImGui.TextDisabled("Please select a SpriteScene to paint anything in this layer.");
        ImGui.EndTooltip();
      }
    }
    Vector2 _mouseStartPaint = Vector2.Zero;
    Vector2 _mouseStartErase = Vector2.Zero;
    bool _isModifying = false;

    public event Action<TileInstance, TileLayer> OnSelectTileInstance;

    public bool IsDoingWork { get => _isModifying; }

    public override void OutRender()
    {
      var rawMouse = InputManager.ScreenMousePosition.ToNumerics();
      var mouse = InputManager.GetWorldMousePosition(Camera);
      SpriteSceneInstance scene;

      if (Nez.Input.LeftMouseButtonPressed)
        _mouseStartErase = InputManager.GetWorldMousePosition(Camera);

      var colorA = _view.Settings.Colors.PaintDefaultFill;
      var colorB = _view.Settings.Colors.PaintDefaultOutline;

      if (_view.PaintMode == PaintMode.None || InputManager.IsImGuiBlocking) return;
      else if (_view.PaintMode == PaintMode.Eraser)
      {
        colorA = _view.Settings.Colors.PaintEraseFill;
        colorB = _view.Settings.Colors.PaintEraseOutline;
      }
      else if (_view.PaintMode == PaintMode.Inspector)
      {
        colorA = _view.Settings.Colors.InspectSpriteFill;
        colorB = _view.Settings.Colors.InspectSpriteOutline;
      }

      if ((!_currentLayer.IsVisible || _currentLayer.IsLocked) && _view.PaintMode != PaintMode.Inspector)
      {
        ImGui.BeginTooltip();
        ImGui.TextDisabled("You cannot paint on this layer.");
        ImGui.EndTooltip();
      }

      if (_currentLayer is TileLayer tileLayer)
      {
        var pos = Camera.WorldToScreenPoint(PosToTiled(mouse));


        if (Nez.Input.LeftMouseButtonPressed) _isModifying = true;

        if (Nez.Input.LeftMouseButtonDown && _view.PaintType == PaintType.Rectangle)
        {
          var drag = new RectangleF(_mouseStartErase, mouse - _mouseStartErase).AlwaysPositive();
          var rect = RectangleF.FromMinMax(PosToTiled(drag.Location), PosToTiled(drag.Max));
          rect.Size = rect.Size.MathMax(tileLayer.TileSize.ToVector2() * Camera.RawZoom);

          ImGui.GetBackgroundDrawList().AddRectFilled(
              Camera.WorldToScreenPoint(rect.Location).ToNumerics(), 
              Camera.WorldToScreenPoint(rect.Max).ToNumerics(), colorA.ToColor().ToImColor());
          ImGui.GetBackgroundDrawList().AddRect(
              Camera.WorldToScreenPoint(rect.Location).ToNumerics(), 
              Camera.WorldToScreenPoint(rect.Max).ToNumerics(), colorB.ToColor().ToImColor());
        } 
 
        if (_view.PaintType == PaintType.Single || (_view.PaintMode == PaintMode.Inspector) && tileLayer.Bounds.Contains(mouse))
        {
          ImGui.GetBackgroundDrawList().AddRectFilled(
              pos.ToNumerics(), (pos + tileLayer.TileSize.ToVector2() * Camera.RawZoom).ToNumerics(), colorA.ToColor().ToImColor());
          ImGui.GetBackgroundDrawList().AddRect(
              pos.ToNumerics(), (pos + tileLayer.TileSize.ToVector2() * Camera.RawZoom).ToNumerics(), colorB.ToImColor()); 

          if (_view.PaintMode == PaintMode.Inspector)
          {
            var tileCoord = tileLayer.GetTileCoordFromWorld(mouse); 
            var tile = tileLayer.TryGetTile(tileCoord);
            var tileName = "Empty";
            if (tile != null)  tileName = (tile.Tile.Name != string.Empty) ? tile.Tile.Name : "No name";
            ImGui.BeginTooltip();
            ImGui.Text($"{tileName} in ({tileCoord.X}, {tileCoord.Y}");
            ImGui.EndTooltip();

            if (Nez.Input.LeftMouseButtonPressed)
            {
              _selectedBounds = new RectangleF(tileCoord.ToVector2() * tileLayer.TileSize.ToVector2() + tileLayer.Bounds.Location, tileLayer.TileSize.ToVector2());
            }
          }
        }

      }
      else if (_currentLayer is FreeformLayer freeform && !_view.Selection.HasBegun())
      {
        var sceneIndex = freeform.GetSceneAt(mouse, out scene);
        if (sceneIndex == -1) return;

        var pos = Camera.WorldToScreenPoint(scene.ContentBounds.Location + scene.Layer.Bounds.Location);
        var size = scene.ContentBounds.Size;

        if (_view.PaintMode != PaintMode.Pen)
        {
          ImGui.GetBackgroundDrawList().AddRectFilled( pos.ToNumerics(), (pos + size * Camera.RawZoom).ToNumerics(), colorA.ToColor().ToImColor());
          ImGui.GetBackgroundDrawList().AddRect( pos.ToNumerics(), (pos + size * Camera.RawZoom).ToNumerics(), colorB.ToImColor()); 
        }

        if (_view.PaintMode == PaintMode.Inspector)
        {
          ImGui.BeginTooltip();
          ImGui.Text($"{scene.Scene.Name} - {sceneIndex}");
          ImGui.EndTooltip();
          if (Nez.Input.LeftMouseButtonPressed)
          {
            _selectedBounds = scene.ContentBounds;
          }

        }
        

      }

      if (Input.LeftMouseButtonReleased) _isModifying = false;
    }
    internal RectangleF _selectedBounds = RectangleF.Empty;
    Utils.TimeDelay _tooltipTimer = new Utils.TimeDelay(2);

    Vector2 PosToTiled(Vector2 pos)
    {
      var layerPos = _currentLayer.Bounds.Location;
      var snapPosInLayer = pos - layerPos;
      var tileLayer = _currentLayer as TileLayer;
      snapPosInLayer = snapPosInLayer.RoundFloor(tileLayer.TileSize);
      snapPosInLayer += layerPos;
      return snapPosInLayer;
    }
    Vector2 PosToTiledRounded(Vector2 pos, Point snap)
    {
      var layerPos = _currentLayer.Bounds.Location;
      var snapPosInLayer = pos - layerPos;
      var tileLayer = _currentLayer as TileLayer;
      snapPosInLayer = snapPosInLayer.RoundFloor(snap);
      snapPosInLayer += layerPos;
      return snapPosInLayer;
    }
        
    Vector2 _mouseStart = Vector2.Zero;
    TileFillFlooder _tileFiller = new TileFillFlooder();
  }
}
