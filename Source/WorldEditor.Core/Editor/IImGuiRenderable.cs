

namespace Raven 
{
  public interface IImGuiRenderable 
  {
    public void Render(ImGuiWinManager imgui);
    
    public bool IsVisible() => true;
  }
}
