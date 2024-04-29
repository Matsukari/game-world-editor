using Nez;
using Microsoft.Xna.Framework;

namespace Raven 
{
  public class Rotator : RenderableComponent, IInputHandler
  {
    public Vector2 Position { get => LocalOffset; }
    public float Angle = -1;
    public float InitialAngle = 0;
    public object Capture;

    bool IInputHandler.OnHandleInput(Raven.InputManager input)
    {
      if (input.IsDragLast)
      {
        Angle = -1;
        Capture = null;
        _isVisible = false;
      }
      return false;
    }

    public void Begin(Vector2 start, Camera camera, object capture)
    {
      LocalOffset = start;
      var pos = camera.MouseToWorldPoint();
      var diff = pos - Position; 
      Angle = MathF.Atan2(diff.Y, diff.X);
      InitialAngle = Angle;
      _isVisible = true;
      Capture = capture;

    }
    public override bool IsVisibleFromCamera(Camera camera) => _isVisible;

    public override void Render(Batcher batcher, Camera camera)
    {
      var pos = camera.MouseToWorldPoint();
      var diff = pos - Position; 
      Angle = MathF.Atan2(diff.Y, diff.X);
      batcher.DrawLineAngle(Position, Angle, MathF.Sqrt(diff.X*diff.X + diff.Y*diff.Y), Color);
    }
      
  }
 
}
