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
    public List<SourcedSprite> Parts = new List<SourcedSprite>();

    /// <summary>
    /// Animations this SpriteScene can do
    /// </summary>
    public List<Animation> Animations = new List<Animation>();

    internal Sheet _sheet;

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
          min.X = Math.Min(min.X, part.Transform.Position.X + part.Origin.X);
          min.Y = Math.Min(min.Y, part.Transform.Position.Y + part.Origin.Y);
          max.X = Math.Max(max.X, part.Transform.Position.X + part.SourceSprite.Region.Size.ToVector2().X + part.Origin.X);
          max.Y = Math.Max(max.Y, part.Transform.Position.Y + part.SourceSprite.Region.Size.ToVector2().Y + part.Origin.Y);
        }
        return RectangleF.FromMinMax(min, max);
      }
    }


    private SpriteScene() 
    {
    }

    public SpriteScene(string name, SourcedSprite main, Sheet sheet) 
    {
      Name = name;
      _sheet = sheet;
      AddSprite("Main component", main);
    }

    public void PutToFront()
    {

    }
    /// <summary>
    /// Creates another set of parts with different instanec
    /// </summary>
    public List<SourcedSprite> DuplicateParts()
    {
      var list = new List<SourcedSprite>();
      foreach (var part in Parts) list.Add(part.Duplicate());
      return list;
    }

    /// <summary>
    /// Adds frame to the given animation
    /// </summary>
    public void AddFrame(string anim, SpriteSceneAnimationFrame frame)
    {
      GetAnimation(anim)?.Frames.Add(frame);
    }

    /// <summary>
    /// Searches for the animation with given name
    /// </summary>
    public Animation GetAnimation(string anim) => Animations.Find((animation)=>animation.Name == anim);

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
    public SourcedSprite AddSprite(string name, SourcedSprite sprite=null) 
    {
      if (sprite == null) sprite = new SourcedSprite();
      if (Parts.Find(item => item.Name == name) != null) 
        name = name.EnsureNoRepeat();

      sprite.SpriteScene = this;
      sprite.Name = name;
      Parts.Add(sprite);
      return sprite;
    }

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
    /// Duplicates the SpriteScene with a new instance of parts but shares the same animation
    /// </summary>
    public SpriteScene Duplicate()
    {
      var spriteScene = new SpriteScene();
      spriteScene.Name = Name.EnsureNoRepeat();
      spriteScene.Parts = DuplicateParts();
      spriteScene.Animations = Animations;
      spriteScene._sheet = _sheet;
      return spriteScene;
    }
  }
}
