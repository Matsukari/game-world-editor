using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet
{
  class RenderableYComparer : IComparer<IRenderable>
  {
    public int Compare(IRenderable self, IRenderable other)
    {
      var res = other.RenderLayer.CompareTo(self.RenderLayer);
      if (res == 0)
      {
        res = other.Bounds.Bottom > self.Bounds.Bottom ? -1 : other.Bounds.Bottom < self.Bounds.Bottom ? 1 : 0;
        return res;
      }
      return res;
    }
  }
  /// <summary>
  /// Arranges painted sprites in a grid-like manner.
  /// Accepts changes in tile size later; cales tiles to fit this layer's tile size
  /// </summary> 
  public class FreeformLayer : Layer
  {
    public bool IsYSorted = true;
    public List<Spritex> Spritexes = new List<Spritex>();
    public FreeformLayer(Level level) : base(level) 
    {
    }
    public Spritex GetSpritexAt(Vector2 pos)
    {
      return Spritexes.FindLast((item)=>item.Bounds.Contains(pos));
    }
    public Spritex PaintSpritex(Spritex spritex)
    {
      var newSpritex = spritex.Clone() as Spritex; 
      Spritexes.Add(newSpritex);
      return newSpritex;
    }
    public void RemoveSpritex(Spritex spritex)
    {
      Spritexes.Remove(spritex);
    }
    public override void Draw(Batcher batcher, Camera camera)
    {
      if (IsYSorted) 
      {
        Spritexes.Sort(new RenderableYComparer());
      }
      foreach (var spritex in Spritexes)
      {
        spritex.Render(batcher, camera);
      }
    }
  }
}
