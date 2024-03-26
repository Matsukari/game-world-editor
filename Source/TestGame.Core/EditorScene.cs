using Nez;

namespace TestGame
{
  public class SpriteSheetEditorScene : Scene
  {
    public override void Initialize()
    {
      base.Initialize();
			SetDesignResolution(1280, 720, SceneResolutionPolicy.None);      
			Screen.SetSize(1280, 720);
      Content.RootDirectory = "Assets";
       
      var editor = AddEntity(new Raven.Editor());

    }   
  }
}
