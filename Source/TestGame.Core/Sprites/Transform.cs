using Microsoft.Xna.Framework;

namespace Raven.Sheet.Sprites 
{
  public struct Transform 
  {
    public Vector2 Position;
    public Vector2 Scale;
    public Vector2 Skew;
    public float Rotation;
    public void Apply(Nez.Transform transform)
    {
      transform.Position = Position;
      transform.Scale = Scale;
      transform.Rotation = Rotation;
    }
  }
}
