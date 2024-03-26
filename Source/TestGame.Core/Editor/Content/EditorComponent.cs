using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet
{
  public class EditorComponent : RenderableComponent, IUpdatable
  {
    public Editor Editor { get => (Editor)Entity; }
    public EditorTabData ContentData { get => Editor.GetContent().Data; }
    public IPropertied Content { get => Editor.GetContent().Content; }
    public Camera Camera { get => Entity.Scene.Camera; }

    virtual public void Update() 
    {
    }
    public override void Render(Batcher batcher, Camera camera) 
    {
    } 
    public virtual void OnContent() 
    {
    }
    // Only works on certain content
    public bool RestrictTo<T>() 
    {
      if (Content is T) Enabled = true;
      else Enabled = false;
      return Enabled;
    }
    public bool RestrictTo(params Type[] types) 
    {
      var match = false;
      foreach (var type in types)
      {
        if (Content.GetType() == type) match = true;
      }
      if (match) Enabled = true;
      else Enabled = false;
      return Enabled;
    }

    // Rudimentary implementation
    public override RectangleF Bounds 
    {
      get
      {
        if (_areBoundsDirty && _bounds.Width != 0)
        {
          _bounds.CalculateBounds(Entity.Position, _localOffset, new Vector2(_bounds.Width, _bounds.Height)/2, 
              Entity.Scale, Entity.Rotation, _bounds.Width, _bounds.Height); 
          _areBoundsDirty = false;
        }
        else _bounds = new RectangleF(-5000, -5000, 10000, 10000);
        return _bounds;
      }
    }       
  }
}
