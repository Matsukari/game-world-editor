using Microsoft.Xna.Framework;
using Nez;
using Nez.Persistence;

namespace Raven 
{
  /// <summary>
  /// SpriteScene is a set of Sprites with a custom render properties and animations. 
  /// </summary>
  public class SpriteScene : IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    /// <summary>
    /// Name to idenity this SpriteScene
    /// </summary>
    public string Name;

    /// <summary>
    /// All the SpriteScene operates on. Like a components that is attached to an Entity
    /// </summary>
    public List<ISceneSprite> Parts = new List<ISceneSprite>();

    /// <summary>
    /// Animations this SpriteScene can do
    /// </summary>
    public List<Animation> Animations = new List<Animation>();

    /// <summary>
    /// Root transform that affects all parts
    /// </summary>
    public Transform Transform = new Transform();

    internal Sheet _sheet;

    public RectangleF Bounds 
    {
      get
      {
        var bounds = EnclosingBounds;
        bounds = bounds.AddPosition(Transform.Position);
        return bounds;
      }
    }

    /// <summary>
    /// Visually, putting a rectangle that exactly fits the bounds of all parts
    /// </summary>
    public RectangleF EnclosingBounds
    {
      get 
      {
        var min = new Vector2(100000, 100000);
        var max = new Vector2(-10000, -10000);
        foreach (var part in Parts)
        {
          min.X = Math.Min(min.X, part.Bounds.X);
          min.Y = Math.Min(min.Y, part.Bounds.Y);
          max.X = Math.Max(max.X, part.Bounds.Right);
          max.Y = Math.Max(max.Y, part.Bounds.Bottom);
        }
        return (Parts.Count() == 0) ? new RectangleF() : RectangleF.FromMinMax(min, max);
      }
    }

    public Vector2 MaxOrigin
    {
      get 
      {
        var max = new Vector2(Int32.MinValue, Int32.MinValue);
        foreach (var part in Parts)  
        {
          max.X = Math.Max(max.X, part.Origin.X);
          max.Y = Math.Max(max.Y, part.Origin.Y);
        }
        return (Parts.Count() == 0) ? Vector2.Zero : max;
      }
    }


    private SpriteScene() 
    {
    }

    public SpriteScene(string name, ISceneSprite main, Sheet sheet) 
    {
      Name = name;
      _sheet = sheet;
      AddSprite("Main component", main);
    }

    /// <summary>
    /// Adds frame to the given animation
    /// </summary>
    public void AddFrame(string anim, SpriteSceneAnimationFrame frame)
    {
      var animation = GetAnimation(anim);
      if (animation != null) 
      {
        animation.Frames.Add(frame);
        animation.Frames = animation.Frames.EnsureNoRepeatNameField();
      }  
    }

    /// <summary>
    /// Searches for the animation with given name
    /// </summary>
    public Animation GetAnimation(string anim) => Animations.Find((animation)=>animation.Name == anim);


    /// <summary>
    /// Removes the animation with given name
    /// </summary>
    public void RemoveAnimation(string anim) 
    {
      var index = Animations.FindIndex(animation => animation.Name == anim);
      if (index != -1)
        Animations.RemoveAt(index);
    }

    /// <summary>
    /// Adds the given animation 
    /// </summary>
    public void AddAnimation(Animation animation) 
    {
      Animations.Add(animation);
      Animations = Animations.EnsureNoRepeatNameField();
    }

    /// <summary>
    /// Inserts a frame after the given index
    /// </summary>
    public void InsertFrame(string anim, int index, SpriteSceneAnimationFrame frame)
    {
      var animation = GetAnimation(anim);
      if (index < 0 || index > animation.TotalFrames) return;
      else if (index == animation.TotalFrames) AddFrame(anim, frame);
      else animation.Frames.Insert(++index, frame);
    }

    /// <summary>
    /// Adds a new part with the name given to this SpriteScene
    /// </summary>
    public ISceneSprite AddSprite(string name, ISceneSprite sprite=null) 
    {
      if (sprite == null) sprite = new SourcedSprite();
      sprite.SpriteScene = this;
      sprite.Name = name;
      Parts.Add(sprite);
      Parts = Parts.EnsureNoRepeatNameField();

      return sprite;
    }
    public ISceneSprite AddSprite(ISceneSprite sprite) => AddSprite((sprite.Name != string.Empty) ? sprite.Name : "Component", sprite);

    /// <summary>
    /// Pushes the given sprite at the given index
    /// </summary>
    public void OrderAt(ISceneSprite sprite, int index) 
    {
      try 
      {
        var current = (Parts.FindIndex(item => item.Name == sprite.Name));

        var temp = Parts[index];
        Parts[index] = Parts[current];
        Parts[current] = temp;
      }
      catch (Exception) {} 
    }
    public void BringDown(ISceneSprite sprite) => OrderAt(sprite, Parts.FindIndex(item => item.Name == sprite.Name) - 1);

    public void BringUp(ISceneSprite sprite) => OrderAt(sprite, Parts.FindIndex(item => item.Name == sprite.Name) + 1); 

    /// <summary>
    /// Removes a part with the same name as the given name
    /// </summary>
    public void RemoveSprite(string name)
    {
      int remove = -1;
      for (int i = 0; i < Parts.Count(); i++) if (Parts[i].Name == name) remove = i;
      if (remove != -1) Parts.RemoveAt(remove);
    }


    /// <summary>
    /// Creates a full copy of SpriteScene with new instances of animations and parts
    /// </summary>
    public SpriteScene Copy()
    {
      var spriteScene = MemberwiseClone() as SpriteScene;
      spriteScene.Properties = Properties.Copy();
      spriteScene.Transform = Transform.Duplicate();
      spriteScene.Parts = Parts.CloneItems();
      spriteScene.Animations = Animations.CloneItems();
      return spriteScene;
    }
  }
}
