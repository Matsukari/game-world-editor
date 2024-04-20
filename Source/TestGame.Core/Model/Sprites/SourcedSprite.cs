using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Persistence;

namespace Raven 
{
  public interface ISceneSprite : IPropertied, ICloneable
  {
    public SpriteScene SpriteScene { get; set; }
    public Sprite SourceSprite { get; set; }

    // Render options 
    public Transform Transform { get; set; }
    public SpriteEffects SpriteEffects { get; set; }
    public Vector2 Origin { get; set; }

    public Vector4 Color { get; set; }

    // Some management options
    public bool IsVisible { get; set; }
    public bool IsLocked { get; set; }

    /// <summary>
    /// Scene bounds plus local bounds. Takes origin
    /// </summary>
    public RectangleF SceneBounds
    { 
      get 
      {
        var bounds = Bounds;
        bounds.Location += SpriteScene.Transform.Position;
        bounds.Size *= SpriteScene.Transform.Scale;
        return bounds;
      }
    }

    /// <summary>
    /// Local bounds; relative to scene. Takes accounts the origin and local scale.
    /// </summary>
    public RectangleF Bounds 
    { 
      get => new RectangleF(
          Transform.Position.X - Origin.X * Transform.Scale.X, 
          Transform.Position.Y - Origin.Y * Transform.Scale.Y, 
          SourceSprite.Region.Width * Transform.Scale.X, 
          SourceSprite.Region.Height * Transform.Scale.Y);
      set 
      {
        Transform.Position = value.Location + Origin;
        Transform.Scale = value.Size / SourceSprite.Region.Size.ToVector2();
      }
    }


    /// <summary>
    /// Local bounds; relative to scene. Takes accounts the origin and local scale
    /// </summary>
    public RectangleF PlainBounds
    { 
      get => new RectangleF(
          Transform.Position.X , 
          Transform.Position.Y , 
          SourceSprite.Region.Width * Transform.Scale.X, 
          SourceSprite.Region.Height * Transform.Scale.Y);
      set 
      {
        Transform.Position = value.Location;
        Transform.Scale = value.Size / SourceSprite.Region.Size.ToVector2();
      }
    }
    public void DetachFromSpriteScene()
    {
      SpriteScene.RemoveSprite(Name);
    }
    public int DeterminePreset()
    {
      Console.WriteLine($"{Origin} == {SourceSprite.Region.Size.ToVector2()/2f}");
      if (Origin == SourceSprite.Region.Size.ToVector2()/2f) return 0;
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
    public virtual ISceneSprite Copy() => Duplicate();

    object ICloneable.Clone() => Copy();
  }
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
    public SpriteScene SpriteScene { get; set; }
    public Sprite SourceSprite { get; set;}

    // Render options 
    public Transform Transform { get; set; } = new Transform(); 
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
    public Vector2 Origin { get; set; } = new Vector2();

    public Vector4 Color { get; set; } = Vector4.One;

    // Some management options
    public bool IsVisible { get; set; } = true;
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
