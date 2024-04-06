
namespace Raven
{
  public interface IImGuiRenderable 
  {
    public void Render(ImGuiWinManager imgui);
  }

  
	public class ImGuiWinManager
  {
    public List<Widget.Window> Windows = new List<Widget.Window>();
    public List<IImGuiRenderable> Renderables = new List<IImGuiRenderable>();
    public Widget.FilePicker FilePicker = new Widget.FilePicker();
    public Widget.NameModal NameModal = new Widget.NameModal();

    public T GetWindow<T>() 
    {
      foreach (var window in Windows) 
      {
        if (window is T windowType) return windowType;
      }
      throw new Exception();
    }
    public void AddIfNotNull(IImGuiRenderable guiRenderable)
    {
      if (guiRenderable != null) Renderables.Add(guiRenderable);
    }
    public void Render()
    {
      foreach (var window in Windows) window.Render(this);
      foreach (var renderable in Renderables) renderable.Render(this);
      NameModal.Draw();
      FilePicker.Draw();
    }
  }
}
