using Nez;

namespace Raven
{
  public abstract class ContentView : EditorInterface
  {
    public virtual IInputHandler InputHandler { get => null; }
    public virtual IImGuiRenderable ImGuiHandler { get => null; }

    public abstract bool CanDealWithType(object content);

    public virtual void OnInitialize(EditorSettings settings) {}
    public virtual void OnContentOpen(IPropertied content) {}
    public virtual void OnContentClose() {}

    public virtual void Render(Batcher batcher, Camera camera, EditorSettings settings) {}

    protected void RenderAnnotations(IPropertied propertied, Batcher batcher, Camera camera, EditorSettings settings)
    {
      batcher.FlushBatch();
      Editor.PrimitiveBatch.Begin(camera.ProjectionMatrix, camera.TransformMatrix);
      foreach (var shape in propertied.Properties)
      {
        if (shape.Value is ShapeModel model)
          model.Render(Editor.PrimitiveBatch, batcher, camera, settings.Colors.ShapeInactive.ToColor()); 
      }
      Editor.PrimitiveBatch.End();
    }

  }
}
