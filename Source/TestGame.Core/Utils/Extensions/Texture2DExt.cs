
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Raven 
{
  public static class Texture2DExt
  {
    public static Vector2 GetSize(this Texture2D texture) => new Vector2(texture.Width, texture.Height);
  }
}
