using Microsoft.Xna.Framework;

namespace Raven.Sheet.Sprites 
{
  public class Transform 
  {
    public Vector2 Position = new Vector2(1, 1);
    public Vector2 Scale = new Vector2(1, 1);
    public Vector2 Skew = new Vector2(1, 1);
    public float Rotation = 0f;
    public Transform() {}
    public void Apply(Nez.Transform transform)
    {
      transform.LocalPosition = Position;
      transform.LocalScale = Scale;
      transform.LocalRotation = Rotation;
    }
  }
}
