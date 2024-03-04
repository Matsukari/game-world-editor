
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Raven.Sheet.Sprites 
{

  public class Sprite : Propertied
  {  
    public int Id = -1;
    public Rectangle Region = new Rectangle();
    public Sprite() {}
  }
  public class Spritex : Propertied
  {  
    public class Sprite : Propertied 
    {
      public Sprites.Sprite SourceSprite;
      public Sprites.Transform Transform; 
      public Vector2 Origin = new Vector2();
      public Sprite() {}
    }
    public struct SpriteMap
    {
      public Dictionary<string, Spritex.Sprite> Data = new Dictionary<string, Spritex.Sprite>();
      public void Add(string name, Spritex.Sprite part) {}
      public SpriteMap() { }
    }
    public SpriteMap Parts = new SpriteMap();
    public Spritex.Sprite MainPart = new Spritex.Sprite();
    public Spritex(string name, Spritex.Sprite main) 
    {
      Name = name;
      MainPart = main;
      MainPart.Name = "Main";
    }
    public RectangleF Bounds 
    {
      get 
      {
        var min = MainPart.Transform.Position;
        var max = MainPart.Transform.Position + MainPart.SourceSprite.Region.Size.ToVector2();
        foreach (var part in Body)
        {
          min.X = Math.Min(min.X, part.Transform.Position.X + part.Origin.X);
          min.Y = Math.Min(min.Y, part.Transform.Position.Y + part.Origin.Y);
          max.X = Math.Max(max.X, part.Transform.Position.X + part.SourceSprite.Region.Size.ToVector2().X + part.Origin.X);
          max.Y = Math.Max(max.Y, part.Transform.Position.Y + part.SourceSprite.Region.Size.ToVector2().Y + part.Origin.Y);
        }
        return RectangleF.FromMinMax(min, max);
      }
    }
    public List<Spritex.Sprite> Body
    {
      get
      {
        var parts = new List<Spritex.Sprite>(Parts.Data.Values);
        parts.Add(MainPart);
        return parts;
      }
    }
  }
}
