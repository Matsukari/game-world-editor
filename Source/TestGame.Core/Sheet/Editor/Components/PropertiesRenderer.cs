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
      // if (ImGui.IsWindowHovered() && Editor.EditState == EditingState.INACTIVE) Editor.Set(EditingState.ACTIVE);
      Editor.SpriteSheet.RenderImGui();
      if (Gui.Selection is IPropertied propertied) propertied.RenderImGui(); 
    }
    public override void OnEditorUpdate()
    {
    }     
  }
}
