
using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{ 
  public class SpriteSceneSpritePicker : EditorInterface
  {
    readonly SpritePicker _picker;

    public object SelectedSprite { get => _picker.SelectedSprite; }
    public event Action<SourcedSprite> OnDropSource; 

    public SpriteSceneSpritePicker(SpritePicker picker)
    {
      _picker = picker;
    }
    bool _isStartDrag = false;
    public void HandleSelectedSprite()
    {
      var input = Core.GetGlobalManager<InputManager>();
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
              screenPos - sprite.Region.GetHalfSize().ToNumerics() * Camera.RawZoom, 
              screenPos - sprite.Region.GetHalfSize().ToNumerics() * Camera.RawZoom + sprite.Region.Size.ToVector2().ToNumerics() 
              * Camera.RawZoom,
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
            var part = new SourcedSprite(sprite);
            part.Transform.Position = Camera.MouseToWorldPoint();
            _picker.SelectedSprite = null;
            OnDropSource(part);
            // var addedSprite = _view.LastSprite.SpriteScene.AddSprite("added-new", new SourcedSprite(_view.LastSprite.SpriteScene, sprite));
          }
        }
      }
    }
  }
}
