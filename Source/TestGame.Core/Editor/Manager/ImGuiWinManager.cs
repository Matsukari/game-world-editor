
namespace Raven
{
  public interface IImGuiRenderable 
  {
    public void Render(ImGuiWinManager imgui);
    
    public bool IsVisible() => true;
  }
 
	public class ImGuiWinManager
  {
    public List<IImGuiRenderable> Renderables = new List<IImGuiRenderable>();
    public Widget.FilePicker FilePicker = new Widget.FilePicker();
    public Widget.NameModal NameModal = new Widget.NameModal();
    List<IImGuiRenderable> _renderablesToAdd = new List<IImGuiRenderable>();
    List<IImGuiRenderable> _renderablesToRemove = new List<IImGuiRenderable>();

    public IImGuiRenderable ContentRenderable;

    public T GetRenderable<T>() 
    {
      foreach (var window in Renderables) 
      {
        if (window is T windowType) return windowType;
      }
      throw new Exception();
    }
    public void AddRenderable(IImGuiRenderable guiRenderable)
    {
      if (guiRenderable != null) 
        _renderablesToAdd.Add(guiRenderable);
    }
    public void RemoveRenderable(IImGuiRenderable guiRenderable)
    {
      if (guiRenderable != null) 
        _renderablesToRemove.Add(guiRenderable);
    }
    public void Render()
    {
      foreach (var renderableToRemove in _renderablesToRemove)
      {
        // Console.WriteLine($"Attempting to remove: {Renderables.Count}");
        Renderables.Remove(renderableToRemove);
        // Console.WriteLine($"After: {Renderables.Count}");
      }
      _renderablesToRemove.Clear();

      foreach (var renderableToAdd in _renderablesToAdd)
      {
        Renderables.Add(renderableToAdd);
      }
      _renderablesToAdd.Clear();

      foreach (var renderable in Renderables) 
      {
        if (renderable.IsVisible())
        {
          renderable.Render(this);
        }
      }

      if (ContentRenderable != null) ContentRenderable.Render(this);
      NameModal.Draw();
      FilePicker.Draw();
    }
  }
}
