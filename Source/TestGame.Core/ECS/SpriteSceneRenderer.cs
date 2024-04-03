using Nez;
using Microsoft.Xna.Framework;

namespace Raven 
{
  public class SpriteSceneRenderer : RenderableComponent 
  {
    public readonly SpriteScene SpriteScene;

    internal SpriteSceneRenderer(SpriteScene scene)
    {
      SpriteScene = scene;
    }
    public override RectangleF Bounds
    {
      get 
      {
        try 
        {
          if (_areBoundsDirty)
          {
            _bounds = SpriteScene.EnclosingBounds;
            _bounds.CalculateBounds(Transform.Position, _localOffset, _bounds.Size/2f, Transform.Scale, Transform.Rotation, _bounds.Width, _bounds.Height);
            _areBoundsDirty = true;
          }
          return _bounds;
        }
        catch (Exception) 
        {
          return SpriteScene.EnclosingBounds;
        }
      }
    }
    public bool GetSpriteComponentAtWorld(Vector2 position)
    {
      for (int i = SpriteScene.Parts.Count()-1; i >= 0; i--)
      {
        var bounds = SpriteScene.Parts[i].Bounds;
        bounds.Location += Transform.Position;
        bounds.Size *= Transform.Scale;
        if (bounds.Contains(position)) return true;
      }
      return false;
    }
    public override void Render(Batcher batcher, Camera camera)
    {
      foreach (var sprite in SpriteScene.Parts)
      {
        batcher.Draw(
            texture: sprite.SourceSprite.Texture,
            position: Transform.Position + LocalOffset + sprite.Transform.Position,
            sourceRectangle: sprite.SourceSprite.Region,
            color: sprite.Color.ToColor(),
            rotation: Transform.Rotation + sprite.Transform.Rotation,
            origin: sprite.Origin,
            scale: Transform.Scale * sprite.Transform.Scale,
            effects: sprite.SpriteEffects,
            layerDepth: _layerDepth);
      }
    }
  }

}
