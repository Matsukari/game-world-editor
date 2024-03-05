using ImGuiNET;
using Nez;
using Nez.ImGuiTools;

namespace Raven.Sheet
{
  public class PropertiesRenderer : Editor.SubEntity
  {
    public override void OnAddedToScene() => Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);    
    public void RenderImGui() 
    {  
      if (Editor.SpriteSheet == null) return;
      Editor.SpriteSheet.RenderImGui();
      if (Gui.Selection is IPropertied propertied) propertied.RenderImGui(); 
    }
  }
}
