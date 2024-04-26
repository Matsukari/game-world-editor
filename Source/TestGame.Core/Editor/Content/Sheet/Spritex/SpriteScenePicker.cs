
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
        if (IsHoverSelected && Nez.Input.LeftMouseButtonDown) _isStartDrag = true;

        if (_isStartDrag && Nez.Input.LeftMouseButtonDown)
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
