using Nez;
using Nez.ImGuiTools;
using ImGuiNET;

namespace Tools
{
  public class SpriteSheetEditorScene : Scene
  {
    public override void Initialize()
    {
      base.Initialize();
			SetDesignResolution(1280, 720, SceneResolutionPolicy.None);      
			Screen.SetSize(1280, 720);
      Content.RootDirectory = "Assets";
       
      var editor = CreateEntity("editor");
      editor.AddComponent(new SpriteSheetEditor());
      // imGuiManager.RegisterDrawCommand(im);
    }   
    void im() {
      ImGui.Begin("Sprite");
      ImGui.Text("ASLDKJLAKSJDLKJSD");
      ImGui.End();
      Console.WriteLine("Im");

    }
  }
}
