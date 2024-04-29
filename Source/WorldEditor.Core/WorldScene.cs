using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{
  public class WorldScene : Scene
  {
    Scene _lastScene;
    World _world; 
    public WorldScene(Scene lastScene, World world, Color background)
    {
      _world = world;
      _lastScene = lastScene;
      ClearColor = background;
    }
    public override void Initialize()
    {
      // SetDesignResolution(1280, 720, SceneResolutionPolicy.None);     
      // Content.RootDirectory = "Assets";
    }    
    public override void OnStart()
    {
      var entity = AddEntity(new WorldEntity(_world));
      entity.AddComponent(new Raven.Utils.Components.CameraMoveComponent());
      entity.AddComponent(new Raven.Utils.Components.CameraZoomComponent());      
    }

    public override void Update()
    {
      base.Update();
      if (Input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape)) 
        Core.StartSceneTransition(new CrossFadeTransition(()=>_lastScene));

    } 
  }
}

