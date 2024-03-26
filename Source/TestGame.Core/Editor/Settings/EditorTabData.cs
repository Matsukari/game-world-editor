using Microsoft.Xna.Framework;

namespace Raven.Sheet
{
  public class EditorTabData
  {
    public Object Selection = null; 
    public Vector2 Position = new Vector2();
    public float Zoom = 1;

    public Shape ShapeSelection = null;
    public IPropertied ShapeContext = null;
  }
}
