using Nez.Persistence;

namespace Raven
{
  public enum EditorContentType
  {
    World,
    Sheet
  }
  public class EditorContent
  {
    public EditorContentData Data { get; private set; }
    public IPropertied Content { get; private set; }

    [JsonExclude]
    public bool Open = true;

    public EditorContent(IPropertied content, EditorContentData data)   
    {
      Content = content;
      Data = data;
    }
  }
}
