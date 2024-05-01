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

    public event Action OnContentChange;

    bool _hasChanges = false;

    [JsonExclude]
    public bool HasChanges 
    {
      get => _hasChanges;
      set 
      {
        _hasChanges = value;
        if (_hasChanges && OnContentChange != null)
          OnContentChange();
      }
    }

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
    public EditorContentData Copy() => MemberwiseClone() as EditorContentData;

    object ICloneable.Clone() 
    {
      return MemberwiseClone();
    }
  }
}
