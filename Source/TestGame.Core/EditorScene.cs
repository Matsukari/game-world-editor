using Nez;

namespace TestGame
{
  public class EditorScene : Scene
  {
    public Raven.Editor Editor;
    public override void Initialize()
    {
      base.Initialize();
			SetDesignResolution(1280, 720, SceneResolutionPolicy.None);      
			Screen.SetSize(1280, 720);
      Content.RootDirectory = "Assets";

      Editor = AddEntity(new Raven.Editor());

    }   
  }
}
