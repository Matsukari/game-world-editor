using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Raven
{
  public class TileLayerRenderer : LayerRenderer<TileLayer>
  { 
    public static void Render(Batcher batcher, Camera camera, TileLayer layer, Transform parent)
    {
      foreach (var (tilePosition, tile) in layer.Tiles)
      {
        var dest = new RectangleF(
            tilePosition.X*layer.TileWidth, 
            tilePosition.Y*layer.TileHeight, 
            layer.TileWidth, layer.TileHeight);

        dest.Location += layer.Bounds.Location;

        var scale = parent.Scale;
        scale.X *= dest.Width / tile.Region.Width;
        scale.Y *= dest.Height / tile.Region.Height;

        var rot = parent.Rotation;
        var eff = SpriteEffects.None;
        var color = Color.White;

        RenderProperties renderProp;
        if (layer.TilesProp.TryGetValue(tilePosition, out renderProp)) 
        {
          rot *= renderProp.Transform.Rotation;
          eff = renderProp.SpriteEffects;
          color = renderProp.Color.ToColor();
        }

        batcher.Draw(
            texture: tile.Texture,
            position: dest.Location + parent.Position,
            sourceRectangle: tile.Region,
            color: color,
            rotation: rot,
            origin: Vector2.Zero,
            scale: scale,
            effects: eff,
            layerDepth: 0);
      }
    }

    public override void Render(Batcher batcher, Camera camera)
    {
      foreach (var (tilePosition, tile) in Layer.Tiles)
      {
        var dest = new RectangleF(
            tilePosition.X*Layer.TileWidth, 
            tilePosition.Y*Layer.TileHeight, 
            Layer.TileWidth, Layer.TileHeight);

        dest.Location += Bounds.Location;

        var scale = Transform.Scale;
        scale.X *= dest.Width / tile.Region.Width;
        scale.Y *= dest.Height / tile.Region.Height;

        var rot = Transform.Rotation;
        var eff = SpriteEffects.None;
        var color = Color.White;

        RenderProperties renderProp;
        if (Layer.TilesProp.TryGetValue(tilePosition, out renderProp)) 
        {
          rot *= renderProp.Transform.Rotation;
          eff = renderProp.SpriteEffects;
          color = renderProp.Color.ToColor();
        }

        batcher.Draw(
            texture: tile.Texture,
            position: dest.Location + Transform.Position,
            sourceRectangle: tile.Region,
            color: color,
            rotation: rot,
            origin: Vector2.Zero,
            scale: scale,
            effects: eff,
            layerDepth: 0);
      }
    }
  } 
}

