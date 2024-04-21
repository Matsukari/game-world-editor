
using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{ 
  public class SpriteSceneSpritePicker : SpritePicker
  {
    public event Action<ISceneSprite> OnDropSource; 
    readonly Camera Camera;
    bool _isStartDrag = false;

    public SpriteSceneSpritePicker(Camera camera) => Camera = camera;
    public override void OnHandleSelectedSprite()
    {
      var input = Core.GetGlobalManager<InputManager>();
      var rawMouse = Nez.Input.RawMousePosition.ToVector2().ToNumerics();

      if (SelectedSprite is Sprite || SelectedSprite is AnimatedSprite)
      {

        // var min = sprite.Region.Location.ToVector2() / sprite.Texture.GetSize();
        // var max = (sprite.Region.Location + sprite.Region.Size).ToVector2() / sprite.Texture.GetSize();
        // var tilePos = rawMouse.ToVector2().RoundFloor(sprite.TileSize).ToNumerics(); 
        // var tilesToPaint = sprite.GetRectTiles();
        //
        // void PaintPreviewAt(System.Numerics.Vector2 screenPos)
        // { 
        //   ImGui.GetForegroundDrawList().AddImage(
        //       Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(sprite.Texture),
        //       screenPos - sprite.Region.GetHalfSize().ToNumerics() * Camera.RawZoom, 
        //       screenPos - sprite.Region.GetHalfSize().ToNumerics() * Camera.RawZoom + sprite.Region.Size.ToVector2().ToNumerics() 
        //       * Camera.RawZoom,
        //       min.ToNumerics(), max.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());
        //
        // }


        if (IsHoverSelected && Nez.Input.LeftMouseButtonDown) _isStartDrag = true;

        if (_isStartDrag && input.IsDrag)
        {
          Sprite sprite = null;
          if (SelectedSprite is Sprite s) sprite = s;
          else if (SelectedSprite is AnimatedSprite s2) sprite = s2.SourceSprite;

          if (sprite != null)
            ImGuiUtils.DrawImage(ImGui.GetForegroundDrawList(), sprite, 
                rawMouse-sprite.Region.GetHalfSize().ToNumerics(), 
                sprite.Region.Size.ToVector2().ToNumerics());

          // Console.WriteLine("Started draggin " + SelectedSprite.GetType().Name);
        }
        else if (_isStartDrag && input.IsDragLast)
        {
          _isStartDrag = false;

          // Pasted in the canvas 
          if (!ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow))
          {
            ISceneSprite part = null;
            if (SelectedSprite is Sprite sprite) part = new SourcedSprite(sprite);
            else if (SelectedSprite is AnimatedSprite anim) part = (anim as ISceneSprite).Copy();
            if (part != null)
            {
              Console.WriteLine("Dropped " + part.GetType().Name);
              part.Transform.Position = Camera.MouseToWorldPoint();
              SelectedSprite = null;
              OnDropSource(part);
            }
          }
        }
      }
    }
  }
}
