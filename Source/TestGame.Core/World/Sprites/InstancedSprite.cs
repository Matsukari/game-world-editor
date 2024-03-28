using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Raven.Sheet.Sprites 
{
  public interface InstancedSprite 
  {
    public PropertyList Properties { get; set; }
    public string Name { get; set; }
    public Transform Transform { get; set; }
    public Color Color { get; set; }
    public SpriteEffects SpriteEffects { get; set; } 
    public bool HasSameSource(InstancedSprite instanced);
  }
  public class TileInstance : InstancedSprite
  {
    public PropertyList Properties { get; set; } = new PropertyList();
    public string Name { get; set; } = "";
    public Transform Transform { get; set; } = new Transform();
    public Color Color { get; set; } = Color.White;
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
    Tile _tile;
    public TileInstance(Tile tile) => _tile = tile;
    
    public bool HasSameSource(InstancedSprite instanced) => _tile.Id == ((TileInstance)instanced)._tile.Id;
    public void Draw(Batcher batcher, Camera camera, RectangleF dest)
    {
      var scale = Transform.Scale;
      scale.X *= dest.Width / _tile.Region.Width;
      scale.Y *= dest.Height / _tile.Region.Height;
      batcher.Draw(
        texture: _tile.Texture,
        position: dest.Location + Transform.Position,
        sourceRectangle: _tile.Region,
        color: Color,
        rotation: Transform.Rotation,
        origin: new Vector2(),
        scale: scale,
        effects: SpriteEffects,
        layerDepth: 0);
    }
  }
  public class SpritexInstance : InstancedSprite
  {
    public PropertyList Properties { get; set; } = new PropertyList();
    public string Name { get; set; } = "";
    public Transform Transform { get; set; } = new Transform();
    public Color Color { get; set; } = Color.White;
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
    Spritex _spritex;
    public Spritex Component { get => _spritex; }
    public void AddToEntity(Entity entity) => entity.AddComponent(_spritex);
    public SpritexInstance(Spritex spritex)
    {
      _spritex = spritex;
    }
    public bool HasSameSource(InstancedSprite instanced) => _spritex.Name == ((SpritexInstance)instanced)._spritex.Name;
    public void Draw(Batcher batcher, Camera camera, RectangleF dest)
    {
      foreach (var sprite in _spritex.Parts)
      {
        batcher.Draw(
            texture: sprite.SourceSprite.Texture,
            position: dest.Location + Transform.Position + _spritex.LocalOffset + sprite.Transform.Position,
            sourceRectangle: sprite.SourceSprite.Region,
            color: sprite.Color,
            rotation: Transform.Rotation + sprite.Transform.Rotation,
            origin: sprite.Origin,
            scale: Transform.Scale * sprite.Transform.Scale,
            effects: sprite.SpriteEffects,
            layerDepth: _spritex.LayerDepth);
      }
      
    }
  }
}
