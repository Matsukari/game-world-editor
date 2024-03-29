using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Raven.Serializers
{

  class SheetSerializer : JsonSerializer<Sheet.Sheet>
  {
    protected override Sheet.Sheet Realize(Sheet.Sheet model)
    {
      model._texture =Texture2D.FromStream(Core.GraphicsDevice, File.OpenRead(model.Filename));
      Insist.IsNotNull(model._texture);  

      Console.WriteLine($"Got spritexs, {model.Spritexes.Count}");
      foreach (var spritex in model.Spritexes)
      {
        Console.WriteLine($"Got, {spritex.Name}");
        spritex._sheet = model;
        foreach (var part in spritex.Parts)
        {
          Console.WriteLine($"Got, {part.Name}");
          Console.WriteLine($"Region, {part.SourceSprite.Region}");
          Console.WriteLine($"{part.IsLocked}");
          Console.WriteLine($"{part.Color}");
          Console.WriteLine($"{part.Origin}");
          Console.WriteLine($"{part.IsVisible}");
          Console.WriteLine($"{part.SpriteEffects}");
          Console.WriteLine($"{part.LocalBounds}");
          part.Spritex = spritex;
          part.SourceSprite._sheet = model;
        }
        foreach (var anim in spritex.Animations)
        {
          anim.Target = spritex;
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

