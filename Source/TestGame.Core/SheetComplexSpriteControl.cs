
using ImGuiNET;
using Num = System.Numerics;
using Nez;

namespace Tools 
{
  public partial class SpriteSheetEditor 
  {
    public partial class SheetComplexSprite : Control
    {
      float _zoom = 1f;
      Num.Vector2 _position = new Num.Vector2();
      public override void Render()
      {
        if (Gui.Selection is ComplexSpriteData complex)
        {
          ImGui.Begin(Names.ComplexSprite + $"{complex.Name}");
          Num.Vector2 min = ImUtils.GetWindowRect().Location.ToVector2().ToNumerics() + _position;
          Num.Vector2 max = ImUtils.GetWindowRect().Location.ToVector2().ToNumerics() + _position + complex.Body.GetArea().Max.ToNumerics();

          ImGui.GetWindowDrawList().AddImage(Gui.SheetTexture, min, max);
          ZoomInput();
          ImGui.End();
        }
      }
      void ZoomInput()
      {
        if (ImGui.GetIO().MouseWheel != 0) 
        {
          var minZoom = 0.2f;
          var maxZoom = 10f;

          var oldSize = _zoom * Editor.SpriteSheet.Size;
          var zoomSpeed = _zoom * 0.2f;
          _zoom += Math.Min(maxZoom - _zoom, ImGui.GetIO().MouseWheel * zoomSpeed);
          _zoom = Mathf.Clamp(_zoom, minZoom, maxZoom);

          // zoom in, move up/left, zoom out the opposite
          var deltaSize = oldSize - (_zoom * Editor.SpriteSheet.Size);
          _position += deltaSize.ToNumerics() * 0.5f;
        }

      }
    }
  }
}
