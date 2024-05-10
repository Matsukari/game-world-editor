using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{
  public class EditorScene : Scene
  {
    public Raven.Editor Editor;
    Point _size = Point.Zero;
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
    public EditorScene(Point size)
    {
      _size = size;
      Editor = AddEntity(new Raven.Editor());
    }
    public EditorScene(Editor editor)
    {
      Editor = editor;
      AddEntity(Editor);
    }
    public EditorScene(Editor editor, Point size)
    {
      _size = size;
      Editor = editor;
      AddEntity(Editor);
    }
    public override void Initialize()
    {
      base.Initialize();
      if (_size != Point.Zero)
      {
        SetDesignResolution(_size.X, _size.Y, SceneResolutionPolicy.None);      
      }
      else 
        SetDesignResolution((int)Screen.PreferredBackBufferWidth-100, (int)Screen.PreferredBackBufferHeight-100, SceneResolutionPolicy.None);      

      Content.RootDirectory = "Assets";
    }   
  }
}
