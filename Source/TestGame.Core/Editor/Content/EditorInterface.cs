using Nez;


namespace Raven
{
  public class EditorInterface
  {
    public Camera Camera { get; private set; }
    public Selection Selection { get; private set; }
    public Guidelines.MovableOriginLines Mover { get; private set; }
    public Rotator Rotator { get; private set; }
    public IPropertied Content { get; private set; }
    public Serializer Serializer { get; private set; }
    public EditorContentData ContentData { get; private set; }
    public EditorSettings Settings { get; private set; }
    public EditorOperator Operator { get => _editor.Operator; }
    public Entity Entity { get; private set; }
    Editor _editor { get => Entity as Editor; }

    public virtual void Initialize(Editor editor, EditorContent content)
    {
      Entity = editor;
      Selection = editor.Selection;
      Camera = editor.Scene.Camera;
      ContentData = content.Data;
      Content = content.Content;
      Serializer = editor.Serializer;
      Settings = editor.Settings;
      Mover = editor.Mover;
      Rotator = editor.Rotator;
    }
    protected void RenderAnnotations(IPropertied propertied)
    {
      foreach (var shape in propertied.Properties)
      {
        if (shape.Value is ShapeModel model)
          model.Render(ImGuiNET.ImGui.GetBackgroundDrawList(), Camera, Settings.Colors.ShapeInactive.ToColor(), Settings.Colors.ShapeOutlineActive.ToColor()); 
      }
    }
    public override bool Equals(object obj)
    {
      if (obj is EditorInterface inf) return Content.Name == inf.Content.Name;
      return false;
    }
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
      
  }
}

