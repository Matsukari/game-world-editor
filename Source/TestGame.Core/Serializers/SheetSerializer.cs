using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Raven.Serializers
{

  class SheetSerializer : JsonSerializer<Sheet>
  {
    protected override Sheet Realize(Sheet model)
    {
      model._texture =Texture2D.FromStream(Core.GraphicsDevice, File.OpenRead(model.Source));
      Insist.IsNotNull(model._texture);  

      foreach (var scene in model.SpriteScenees)
      {
        scene._sheet = model;
        foreach (var part in scene.Parts)
        {
          part.SpriteScene = scene;
          part.SourceSprite._sheet = model;
        }
        foreach (var anim in scene.Animations)
        {
          anim.Target = scene;
          foreach (var frame in anim.Frames)
          {
            if (frame is SpriteSceneAnimationFrame sceneFrame) 
            {
              foreach (var part in sceneFrame.Parts) 
              {
                part.SpriteScene = scene;
                part.SourceSprite._sheet = model;
              }
            }
          }
        }
      }
      foreach (var tile in model.TileMap)
      {
        tile.Value._sheet = model;
      }
      return model;
    }
  }
}

