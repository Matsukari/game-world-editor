
namespace Raven.Sheet.Sprites 
{
  public class SpriteMap 
  {
    public Dictionary<string, SourcedSprite> Data = new Dictionary<string, SourcedSprite>();
    public void Add(string name, SourcedSprite part) { Data.TryAdd(name, part); part.Name = name; }
    public SpriteMap() { }
    public SpriteMap Duplicate()
    {
      SpriteMap spriteMap = new SpriteMap();
      foreach (var sprite in Data) spriteMap.Data.Add(sprite.Key, sprite.Value.Duplicate());
      return spriteMap;
    }

  }
}
