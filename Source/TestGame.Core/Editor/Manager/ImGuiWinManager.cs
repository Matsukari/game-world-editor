
namespace Raven
{
 
	public class ImGuiWinManager
  {
    public class Entry
    {
      public string Tag = "";
      public IImGuiRenderable Renderable;
      public Entry(IImGuiRenderable renderable) => Renderable = renderable;
    }
    public List<Entry> Renderables = new List<Entry>();

    public Widget.FilePicker FilePicker = new Widget.FilePicker();
    public Widget.NameModal NameModal = new Widget.NameModal();
    public Widget.ConfirmModal ConfirmModal = new Widget.ConfirmModal();

    List<IImGuiRenderable> _entrysToAdd = new List<IImGuiRenderable>();
    List<IImGuiRenderable> _entrysToRemove = new List<IImGuiRenderable>();

    public void AddImmediate(IImGuiRenderable renderable, string tag="")
    {
      var entry = new Entry(renderable);
      entry.Tag = tag;
      Renderables.Add(entry);
    }
    public T GetRenderable<T>(string tag) where T: IImGuiRenderable => (T)Renderables.Find(item => item.Tag == tag).Renderable;

    public T GetRenderable<T>() 
    {
      foreach (var entry in Renderables) 
      {
        if (entry.Renderable is T windowType) return windowType;
      }
      throw new Exception();
    }
    public void AddRenderable(IImGuiRenderable guiRenderable)
    {
      if (guiRenderable != null) 
        _entrysToAdd.Add(guiRenderable);
    }
    public void RemoveRenderable(IImGuiRenderable guiRenderable)
    {
      if (guiRenderable != null) 
        _entrysToRemove.Add(guiRenderable);
    }
    public void Render()
    {
      foreach (var entryToRemove in _entrysToRemove)
      {
        // Console.WriteLine($"Attempting to remove: {Renderables.Count}");
        Renderables.RemoveAll(item => item.Renderable == entryToRemove);
        // Console.WriteLine($"After: {Renderables.Count}");
      }
      _entrysToRemove.Clear();

      foreach (var entryToAdd in _entrysToAdd)
      {
        if (Renderables.Find(item => item.Renderable == entryToAdd) != null) continue;
        Renderables.Add(new Entry(entryToAdd));
      }
      _entrysToAdd.Clear();

      foreach (var entry in Renderables) 
      {
        if (entry.Renderable.IsVisible())
        {
          entry.Renderable.Render(this);
        }
      }

      FilePicker.Draw(this);
      NameModal.Draw();
      ConfirmModal.Draw();
    }
  }
}
