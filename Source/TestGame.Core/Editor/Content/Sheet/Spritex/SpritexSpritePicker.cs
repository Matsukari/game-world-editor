
using ImGuiNET;
using Nez;
using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;

namespace Raven.Sheet
{ 
  public class SpritexSpritePicker
  {
    public object SelectedSprite { get => _picker.SelectedSprite; }
    SpritexView _view;
    SpritePicker _picker { get => _view.Editor.GetEditorComponent<SheetView>()._inspector.SpritePicker; }
    public SpritexSpritePicker(SpritexView view)
    {
      _view = view;
    }
    bool _isStartDrag = false;
    public void HandleSelectedSprite()
    {
      var input = Core.GetGlobalManager<Input.InputManager>();
      var rawMouse = Nez.Input.RawMousePosition.ToVector2().ToNumerics();

      if (SelectedSprite is Sprite sprite)
      {
        var min = sprite.Region.Location.ToVector2() / sprite.Texture.GetSize();
        var max = (sprite.Region.Location + sprite.Region.Size).ToVector2() / sprite.Texture.GetSize();
        var tilePos = rawMouse.ToVector2().RoundFloor(sprite.TileSize).ToNumerics(); 
        var tilesToPaint = sprite.GetRectTiles();

        void PaintPreviewAt(System.Numerics.Vector2 screenPos)
        { 
          ImGui.GetForegroundDrawList().AddImage(
              Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(sprite.Texture),
              screenPos - sprite.Region.GetHalfSize().ToNumerics() * _view.Entity.Scene.Camera.RawZoom, 
              screenPos - sprite.Region.GetHalfSize().ToNumerics() * _view.Entity.Scene.Camera.RawZoom + sprite.Region.Size.ToVector2().ToNumerics() 
              * _view.Entity.Scene.Camera.RawZoom,
              min.ToNumerics(), max.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());

        }
        if (_picker.IsHoverSelected && Nez.Input.LeftMouseButtonDown) _isStartDrag = true;

        if (_isStartDrag && input.IsDrag)
        {
          PaintPreviewAt(rawMouse);
        }
        else if (_isStartDrag && input.IsDragLast)
        {
          _isStartDrag = false;
          // Pasted in the canvas 
          if (!ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow))
          {
            var addedSprite = _view.LastSprite.Spritex.AddSprite("added-new", new SourcedSprite(_view.LastSprite.Spritex, sprite));
            addedSprite.Transform.Position = _view.Entity.Scene.Camera.MouseToWorldPoint();
            _picker.SelectedSprite = null;
          }
        }
      }
    }
  }
}
