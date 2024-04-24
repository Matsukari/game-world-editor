
namespace Raven
{
  public class EditorSettings
  {
    public EditorColors Colors = new EditorColors();

    public EditorGraphics Graphics = new EditorGraphics();

    public EditorHotkeys Hotkeys = new EditorHotkeys();

    public List<EditorContentData> LastFiles = new List<EditorContentData>();

    public int LastFile = 0;

    public bool IsEditorBusy = false;
  } 
}
