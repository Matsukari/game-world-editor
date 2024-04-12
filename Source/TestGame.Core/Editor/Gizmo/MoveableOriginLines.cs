using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Guidelines
{
  public class MovableOriginLines : RenderableComponent, IInputHandler
  {
    public enum AxisType { X, Y, None };
    public Vector2 Length = new Vector2(50, 50);
    public Vector2 Thickness = new Vector2(5, 5);
    public float MinThickness = 5f;
    public float MaxThickness = 10f;
    public Color HorizontalColor { get => Color; set => Color = value; }
    public Color VerticalColor;
    public (Vector2, Vector2) LineX { get => (LocalOffset, new Vector2(LocalOffset.X + Length.X, LocalOffset.Y)); }
    public (Vector2, Vector2) LineY { get => (LocalOffset, new Vector2(LocalOffset.X, LocalOffset.Y + Length.Y)); }
    public AxisType Axis = AxisType.None;
    public Vector2 Position { get => LocalOffset; }
    public Vector2 InitialPosition = Vector2.Zero;
    public object Capture;

    public MovableOriginLines() 
    {
      HorizontalColor = Color.Red;
      VerticalColor = Color.Green;
      Hide();
    }

    public Vector2 AbsoluteLength((Vector2, Vector2) line, Camera camera) 
    {
      var diff = line.Item2 - line.Item1;
      return line.Item1 + diff / camera.RawZoom;
    }

    bool IInputHandler.OnHandleInput(Raven.InputManager input)
    {
      if (input.IsDragLast) 
      {
        Axis = AxisType.None;
        Capture = null;
      }
      Collides(input.Camera);

      var pos = LocalOffset;
      if (Axis == AxisType.X)
      {
        pos.X = InitialPosition.X + (input.Camera.MouseToWorldPoint().X - InitialPosition.X);
        Thickness.X = MaxThickness;
        Thickness.Y = MinThickness;
      }
      else if (Axis == AxisType.Y)
      {
        pos.Y = InitialPosition.Y + (input.Camera.MouseToWorldPoint().Y - InitialPosition.Y);
        Thickness.X = MinThickness;
        Thickness.Y = MaxThickness;
      }
      LocalOffset = pos;

      return Axis != AxisType.None;
    }
    public AxisType Collides(Camera camera)
    {
      if (Collisions.CircleToLine(camera.MouseToWorldPoint(), Thickness.X/camera.RawZoom, LineX.Item1, AbsoluteLength(LineX, camera))) 
      {
        Thickness.X = MaxThickness;
        Thickness.Y = MinThickness;
        return AxisType.X;
      }
      else if (Collisions.CircleToLine(camera.MouseToWorldPoint(), Thickness.Y/camera.RawZoom, LineY.Item1, AbsoluteLength(LineY, camera))) 
      {
        Thickness.X = MinThickness;
        Thickness.Y = MaxThickness;
        return AxisType.Y;
      }
      Thickness = new Vector2(MinThickness, MinThickness);
      return AxisType.None;
    }
    public void TryBegin(Vector2 start, Camera camera, object capture)
    {
      var axis = Collides(camera);
      if (axis != AxisType.None) Begin(start, axis, capture);
    }
    public void Begin(Vector2 start, AxisType axis, object capture)
    {
      InitialPosition = start;
      Capture = capture;
      Axis = axis;
    }

    public void Show(Vector2 start) 
    {
      _isVisible = true;
      LocalOffset = start;
    }
    public void Hide() => _isVisible = false;

    public override bool IsVisibleFromCamera(Camera camera) => _isVisible;
        
    public override void Render(Batcher batcher, Camera camera)
    {
      batcher.DrawLine(LineX.Item1, AbsoluteLength(LineX, camera), HorizontalColor,  thickness: Thickness.X/camera.RawZoom);
      batcher.DrawLine(LineY.Item1, AbsoluteLength(LineY, camera), VerticalColor, thickness: Thickness.Y/camera.RawZoom);
    }
  }

}
