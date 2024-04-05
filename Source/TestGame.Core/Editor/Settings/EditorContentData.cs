using Microsoft.Xna.Framework;

namespace Raven
{
  public class EditorContentData
  {
    public string Filename;
    public string Type;

    public float Zoom = 1f;
    public Vector2 Position = Vector2.Zero;

    public IPropertied PropertiedContext = null;
    public SelectionList SelectionList = new SelectionList();

    public EditorContentData(string filename, string type)
    {
      Filename = filename;
      Type = type;
    }
  }
}
