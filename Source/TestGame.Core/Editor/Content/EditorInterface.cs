using Nez;


namespace Raven
{
  public class EditorInterface
  {
    public Camera Camera { get; private set; }
    public Selection Selection { get; private set; }
    public IPropertied Content { get; private set; }
    public Serializer Serializer { get; private set; }
    public EditorContentData ContentData { get; private set; }
    public EditorSettings Settings { get; private set; }
    public Entity Entity { get; private set; }

    public virtual void Initialize(Editor editor)
    {
      Entity = editor;
      Selection = editor.Selection;
      Camera = editor.Scene.Camera;
      ContentData = editor.ContentManager.ContentData;
      Content = editor.ContentManager.Content;
      Serializer = editor.Serializer;
      Settings = editor.Settings;
    }
  }
}

