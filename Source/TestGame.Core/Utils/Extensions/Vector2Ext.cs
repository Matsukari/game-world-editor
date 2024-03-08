using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Num = System.Numerics;
using Nez;
using Nez.ImGuiTools;
using ImGuiNET;

namespace Raven
{
  public static class Vector2Ext 
  {
    public static Vector2 ToVector2(this Num.Vector2 numeric)
    {
      return new Vector2(numeric.X, numeric.Y);
    }
  }
}
