using Nez;

namespace Raven 
{
  /// <summary>
  /// Sorts the Renderables based on their Y corrdinates; bottom are in the front, upper is sent to back
  /// </summary>
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
}
