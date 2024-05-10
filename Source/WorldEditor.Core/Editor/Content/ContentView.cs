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
    public virtual void OnContentOpen(ImGuiWinManager imgui) {}
    public virtual void OnContentClose() {}
    public virtual void OnContentClose(ImGuiWinManager imgui) {}

    public virtual void Render(Batcher batcher, Camera camera, EditorSettings settings) {}


  }
}
