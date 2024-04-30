using Nez;

namespace Raven
{
  public class EditorScene : Scene
  {
    public Raven.Editor Editor;
    bool _once = true;

    public override void Begin()
    {
      if (_once)
        base.Begin();
      else 
        Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().RegisterDrawCommand(Editor.WindowManager.Render);

      _once = false;
    }
    public override void End() {}

    public EditorScene()
    {
      Editor = AddEntity(new Raven.Editor());
    }
    public EditorScene(Editor editor)
    {
      Editor = editor;
      AddEntity(Editor);
    }
    public override void Initialize()
    {
      base.Initialize();
			SetDesignResolution(1280, 720, SceneResolutionPolicy.None);      
			Screen.SetSize(1280, 720);
      Content.RootDirectory = "Assets";
    }   
  }
}