using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Persistence;

namespace Raven 
{
   /// <summary>
  /// A render model with a Transform and other basic render options 
  /// and a reference to a sprite in a Sheet.
  /// </summary>
  public class SourcedSprite : IPropertied, ISceneSprite
  {
    string IPropertied.Name { get => Name; set => Name = value; }

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    public string Name = "";

    [JsonInclude]
    public SpriteScene SpriteScene { get; set; }

    [JsonInclude]
    public Sprite SourceSprite { get; set;}

    // Render options 
    [JsonInclude]
    public Transform Transform { get; set; } = new Transform(); 

    [JsonInclude]
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

    [JsonInclude]
    public Vector2 Origin { get; set; } = new Vector2();

    [JsonInclude]
    public Vector4 Color { get; set; } = Vector4.One;

    // Some management options
    [JsonInclude]
    public bool IsVisible { get; set; } = true;

    [JsonInclude]
    public bool IsLocked { get; set; } = false;


    private SourcedSprite()
    {
    }

    public SourcedSprite(SpriteScene spriteScene=null, Sprite sprite=null) 
    {
      SpriteScene = spriteScene;
      SourceSprite = sprite;
    }
    public SourcedSprite(Sprite sprite) 
    {
      SourceSprite = sprite;
    }
  }
}
