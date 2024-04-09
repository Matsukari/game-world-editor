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
    /// Removes the animation with given name
    /// </summary>
    public void RemoveAnimation(string anim) 
    {
      var index = Animations.FindIndex(animation => animation.Name == anim);
      if (index != -1)
        Animations.RemoveAt(index);
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
    public SourcedSprite AddSprite(SourcedSprite sprite) => AddSprite("Component", sprite);

    /// <summary>
    /// Pushes the given sprite at the given index
    /// </summary>
    public void OrderAt(SourcedSprite sprite, int index) 
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
    public void BringDown(SourcedSprite sprite) => OrderAt(sprite, Parts.FindIndex(item => item.Name == sprite.Name) - 1);

    public void BringUp(SourcedSprite sprite) => OrderAt(sprite, Parts.FindIndex(item => item.Name == sprite.Name) + 1); 

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
    /// Duplicates the SpriteScene with a new instance of parts but shares the same animation and properties
    /// </summary>
    public SpriteScene Duplicate()
    {
      var spriteScene = MemberwiseClone() as SpriteScene;
      spriteScene.Name = Name.EnsureNoRepeat();
      spriteScene.Parts = DuplicateParts();
      spriteScene.Animations = Animations;
      return spriteScene;
    }

    /// <summary>
    /// Creates a full copy of SpriteScene with new instances of animations and parts
    /// </summary>
    public SpriteScene Copy()
    {
      var spriteScene = new SpriteScene();
      spriteScene.Name = Name.EnsureNoRepeat();
      spriteScene.Properties = Properties.Copy();
      spriteScene.Parts = DuplicateParts();
      spriteScene.Animations = Animations.CloneItems();
      spriteScene._sheet = _sheet;
      return spriteScene;
    }
  }
}
