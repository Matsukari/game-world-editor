using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Raven
{
  public class TileLayerRenderer : LayerRenderer<TileLayer>
  { 
    public static void Render(Batcher batcher, Camera camera, TileLayer layer, Transform parent, Color globalColor=default, Color nameColor=default)
    {
      if (globalColor == default) globalColor = Color.White;
      if (nameColor == default) nameColor = Color.White;

      var names = new List<(Point, TileInstance)>();

      foreach (var (tilePosition, tileInstance) in layer.Tiles)
      {
        var tile = tileInstance.Tile;

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
        var color = globalColor;

        RenderProperties renderProp;
        if (tileInstance.Props != null) 
        {
          renderProp = tileInstance.Props;
          rot *= renderProp.Transform.Rotation;
          eff = renderProp.SpriteEffects;
          color = color.Average(renderProp.Color.ToColor());
        }

        if (tileInstance.Props != null)
        {
          dest.Location += tileInstance.Props.Transform.Position;
          rot += tileInstance.Props.Transform.Rotation;
          scale *= tileInstance.Props.Transform.Scale;

        }
        names.Add((tilePosition, tileInstance));

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
      foreach (var (tilePosition, tileInstance) in names)
      {
        var tile = tileInstance.Tile;
        var dest = new RectangleF(
            tilePosition.X*layer.TileWidth, 
            tilePosition.Y*layer.TileHeight, 
            layer.TileWidth, layer.TileHeight);

        dest.Location += layer.Bounds.Location;

        var scale = parent.Scale;
        scale.X *= dest.Width / tile.Region.Width;
        scale.Y *= dest.Height / tile.Region.Height;

        if (tileInstance.Props != null) dest.Location += tileInstance.Props.Transform.Position;
        batcher.DrawStringCentered(camera, tileInstance.Name, dest.Center+parent.Position, nameColor, new Vector2(1, 5), true, true);
      }
    }

    public override bool IsVisibleFromCamera(Camera camera)
    {
      return base.IsVisibleFromCamera(camera) && Layer.IsVisible;
    }
        
    public override void Render(Batcher batcher, Camera camera)
    {
      Console.WriteLine("Rendering TileLayer...");
      foreach (var (tilePosition, tileInstance) in Layer.Tiles)
      {
        var tile = tileInstance.Tile;
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
        if (tileInstance.Props != null) 
        {
          renderProp = tileInstance.Props;
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

