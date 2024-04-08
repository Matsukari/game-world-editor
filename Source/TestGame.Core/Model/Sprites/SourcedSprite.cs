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
  public class SourcedSprite : IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    public string Name = "";
    public SpriteScene SpriteScene;
    public Sprite SourceSprite;

    // Render options 
    public Transform Transform = new Transform(); 
    public SpriteEffects SpriteEffects;
    public Vector2 Origin = new Vector2();

    public Vector4 Color = Vector4.One;

    // Some management options
    public bool IsVisible = true;
    public bool IsLocked = false;

    // Local bounds
    public RectangleF Bounds 
    { 
      get => new RectangleF(
          Transform.Position.X - Origin.X * Transform.Scale.X, 
          Transform.Position.Y - Origin.Y * Transform.Scale.Y, 
          SourceSprite.Region.Width * Transform.Scale.X, 
          SourceSprite.Region.Height * Transform.Scale.Y);
    }
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
    public void DetachFromSpriteScene()
    {
      SpriteScene.RemoveSprite(Name);
    }
    public int DeterminePreset()
    {
      if (Origin == Bounds.Size/2f) return 0;
      else if (Origin == Vector2.Zero) return 1;
      else return 2;
    }
    public SourcedSprite Duplicate()
    {
      SourcedSprite sprite = new SourcedSprite();
      sprite.Name = Name;
      sprite.Properties = Properties.Copy();
      sprite.SpriteScene = SpriteScene;
      sprite.SourceSprite = SourceSprite;
      sprite.Transform = Transform.Duplicate();
      sprite.SpriteEffects = SpriteEffects;
      sprite.Origin = Origin;
      sprite.Color = Color;
      return sprite;
    }
  }
}
