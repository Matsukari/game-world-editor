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


ImGui.GetWindowDrawList().AddRectFilled(rect, Editor.ColorSet.AnnotatedShape, Gui.);

void DrawGridLines(int tw, int th)
    {
      float w = tw * ContentZoom;
      float h = th * ContentZoom;
      var (min, max) = ImUtils.GetWindowArea();
      var drawList = ImGui.GetWindowDrawList();
      int cols = (int)(_textureSize.X / tw);
      int rows = (int)(_textureSize.Y / th);
      uint color = ImGui.ColorConvertFloat4ToU32(new Num.Vector4(0.3f, 0.3f, 0.3f, 0.3f));
      for (int x = 0; x <= cols; x++) 
      { 
        float xx = x;
        drawList.AddLine(
            min + new Num.Vector2(_imagePosition.X + xx * w, _imagePosition.Y), 
            min + new Num.Vector2(_imagePosition.X + xx * w, _imagePosition.Y + rows * h), color);
        for (int y = 0; y <= rows; y++) 
        {
          float yy = y;
          drawList.AddLine(
              min + new Num.Vector2(_imagePosition.X, _imagePosition.Y + y * h), 
              min + new Num.Vector2(_imagePosition.X + cols * w, _imagePosition.Y + y * h), color);
        }
      }
    }


    void DrawPopups()
    {
      if (ImGui.GetIO().MouseReleased[1] && _selSprite is TiledSpriteData) 
      {
        ImGui.OpenPopup("sprite-editing", ImGuiPopupFlags.NoOpenOverExistingPopup);
        isSpriteOptions = true;
      }
      if (isSpriteOptions && _selSprite is TiledSpriteData tile)
      {
        ImGui.BeginPopup("sprite-editing");
        if (ImGui.MenuItem("Create ComplexSprite")) SpriteSheet.AddSprite(tile);
        ImGui.EndPopup();
      }
    }
