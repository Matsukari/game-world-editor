using Nez;
using ImGuiNET;

namespace TestGame
{
  public class EditorScene : Scene
  {
    public override void Initialize()
    {
      base.Initialize();
			SetDesignResolution(1280, 720, SceneResolutionPolicy.None);      
			Screen.SetSize(1280, 720);
      Content.RootDirectory = "Assets";
       
      var editor = AddEntity(new Raven.Editor());

      if (editor.Settings.ImGuiColors.Count != ImGui.GetStyle().Colors.Count)
      {
        for (int i = 0; i < ImGui.GetStyle().Colors.Count; i++)
          editor.Settings.ImGuiColors.Add(ImGui.GetStyle().Colors[i]);
      }
    }   
  }
}
