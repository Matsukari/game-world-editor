using Microsoft.Xna.Framework.Graphics;
using Nez;
using Raven.Sheet;

namespace Raven.Serializers
{

  class SheetSerializer : JsonSerializer<Sheet.Sheet>
  {
    protected override Sheet.Sheet Realize(Sheet.Sheet model)
    {
      model._texture =Texture2D.FromStream(Core.GraphicsDevice, File.OpenRead(model.Filename));
      Insist.IsNotNull(model._texture);  

      foreach (var spritex in model.Spritexes)
      {
        spritex._sheet = model;
        foreach (var part in spritex.Parts)
        {
          part.Spritex = spritex;
          part.SourceSprite._sheet = model;
        }
        foreach (var anim in spritex.Animations)
        {
          anim.Target = spritex;
          foreach (var frame in anim.Frames)
          {
            if (frame is SpritexAnimationFrame spritexFrame) 
            {
              foreach (var part in spritexFrame.Parts) 
              {
                part.Spritex = spritex;
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

