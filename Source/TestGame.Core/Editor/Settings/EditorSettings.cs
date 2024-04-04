
namespace Raven
{
  public class EditorSettings
  {
    public EditorColors Colors = new EditorColors();
    public List<EditorContentData> LastFiles = new List<EditorContentData>();
    public int LastFile = 0;
    public bool IsEditorBusy = false;
  } 
}
