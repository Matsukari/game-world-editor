using Microsoft.Xna.Framework;
using Nez.Persistence;

namespace Raven
{
  public class EditorContentData : ICloneable
  {
    public string Filename;
    public string Type;

    public float Zoom = 1f;
    public Vector2 Position = Vector2.Zero;

    public EditorContentData Copy() => MemberwiseClone() as EditorContentData;

    [JsonExclude]
    public IPropertied PropertiedContext = null;

    [JsonExclude]
    public SelectionList SelectionList = new SelectionList();

    private EditorContentData()
    {}

    public EditorContentData(string filename, string type)
    {
      Filename = filename;
      Type = type;
    }

    object ICloneable.Clone() 
    {
      return MemberwiseClone();
    }
  }
}
