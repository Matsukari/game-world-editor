
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Raven
{
  public class RenderProperties
  {
    public Transform Transform = new Transform();
    public Vector4 Color = Vector4.One;
    public SpriteEffects SpriteEffects = SpriteEffects.None;
    public RenderProperties Copy() 
    {
      var ren = MemberwiseClone() as RenderProperties;
      ren.Transform = Transform.Duplicate();
      return ren;
    }
  }
}
