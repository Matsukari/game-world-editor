using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Persistence;

namespace Raven.Sheet.Sprites 
{
  /// <summary>
  /// A render model with a Transform and other basic render options 
  /// and a reference to a sprite in a Sheet.
  /// </summary>
  public class SourcedSprite : IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }
    public PropertyList Properties { get; set; } = new PropertyList();

    public string Name = "";
    public Spritex Spritex;
    public Sprite SourceSprite;

    // Render options 
    public Sprites.Transform Transform = new Transform(); 
    public SpriteEffects SpriteEffects;
    public Vector2 Origin = new Vector2();

    [JsonInclude]
    public Color Color = Color.White;

    // Some management options
    public bool IsVisible = true;
    public bool IsLocked = false;

    // Local bounds
    public RectangleF LocalBounds { 
      get => new RectangleF(
          Transform.Position.X - Origin.X * Transform.Scale.X, 
          Transform.Position.Y - Origin.Y * Transform.Scale.Y, 
          SourceSprite.Region.Width * Transform.Scale.X, 
          SourceSprite.Region.Height * Transform.Scale.Y);
    }

    public RectangleF WorldBounds { 
      get 
      {
        var bounds = LocalBounds;
        bounds.Location += Spritex.Transform.Position;
        bounds.Size *= Spritex.Transform.Scale;
        return bounds;
      }
    }
    private SourcedSprite()
    {}
    public SourcedSprite(Spritex spritex=null, Sprites.Sprite sprite=null) 
    {
      Spritex = spritex;
      SourceSprite = sprite;
    }
    public void DetachFromSpritex()
    {
      Spritex.RemoveSprite(Name);
    }
    public int DeterminePreset()
    {
      if (Origin == LocalBounds.Size/2f) return 0;
      else if (Origin == Vector2.Zero) return 1;
      else return 2;
    }
    public SourcedSprite Duplicate()
    {
      SourcedSprite sprite = new SourcedSprite();
      sprite.Spritex = Spritex;
      sprite.SourceSprite = SourceSprite;
      sprite.Transform = Transform.Duplicate();
      sprite.SpriteEffects = SpriteEffects;
      sprite.Origin = Origin;
      sprite.Color = Color;
      return sprite;
    }
  }
}
