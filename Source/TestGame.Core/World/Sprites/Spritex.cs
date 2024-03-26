using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet.Sprites 
{
  public class Spritex : RenderableComponent, IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }
    public PropertyList Properties { get; set; } = new PropertyList();

    internal Sheet _sheet;
    public string Name;
    public SpriteMap Parts = new SpriteMap();
    public List<SourcedSprite> Body { get => new List<SourcedSprite>(Parts.Data.Values); } 

    public Spritex(string name, SourcedSprite main, Sheet sheet) 
    {
      Name = name;
      Parts.Add(name, main);
      main.Spritex = this;
      _sheet = sheet;
    }
    public RectangleF EnclosingBounds
    {
      get 
      {
        var min = new Vector2(100000, 100000);
        var max = new Vector2(-10000, -10000);
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
    public override RectangleF Bounds
    {
      get 
      {
        try 
        {
          if (_areBoundsDirty)
          {
            _bounds = EnclosingBounds;
            _bounds.CalculateBounds(Transform.Position, _localOffset, _bounds.Size/2f, Transform.Scale, Transform.Rotation, _bounds.Width, _bounds.Height);
            _areBoundsDirty = true;
          }
          return _bounds;
        }
        catch (Exception _) 
        {
          return EnclosingBounds;
        }
      }
    }
    public SourcedSprite AddSprite(string name, SourcedSprite sprite=null) 
    {
      if (sprite == null)
      {
        sprite = new SourcedSprite();
      }
      sprite.Spritex = this;
      Parts.Add(name, sprite);
      return sprite;
    }
    public override void Render(Batcher batcher, Camera camera)
    {
      foreach (var sprite in Body)
      {
        batcher.Draw(
            texture: sprite.SourceSprite.Texture,
            position: Transform.Position + LocalOffset + sprite.Transform.Position,
            sourceRectangle: sprite.SourceSprite.Region,
            color: sprite.Color,
            rotation: Transform.Rotation + sprite.Transform.Rotation,
            origin: sprite.Origin,
            scale: Transform.Scale * sprite.Transform.Scale,
            effects: sprite.SpriteEffects,
            layerDepth: _layerDepth);
      }
    }
    public override Component Clone()
    {
      Spritex spritex = base.Clone() as Spritex;
      spritex.Entity = Entity;
      spritex._sheet = _sheet;
      spritex.Parts = Parts.Duplicate();
      return spritex;
    }
        

  }
}
