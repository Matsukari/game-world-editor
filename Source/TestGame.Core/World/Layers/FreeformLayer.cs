using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet
{
  public class EntityLayer : Layer
  {
    public EntityLayer(Level level) : base(level) {}
    public List<Spritex> Spritexes;
  } 
}
