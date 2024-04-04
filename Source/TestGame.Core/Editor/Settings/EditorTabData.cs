using Microsoft.Xna.Framework;

namespace Raven
{
  public class EditorContentData
  {
    public string Filename;
    public string Type;
    public Vector2 Position = Vector2.Zero;
    public float Zoom = 1f;
    public List<object> Selections = new List<object>();
    public EditorContentData(string filename, string type)
    {
      Filename = filename;
      Type = type;
    }
  }
}
