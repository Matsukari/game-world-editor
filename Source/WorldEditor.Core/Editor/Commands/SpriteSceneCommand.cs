

namespace Raven
{
  class AddSpriteSceneCommand : Command
  {
    internal readonly Sheet _sheet;
    SpriteScene _scene;

    public AddSpriteSceneCommand(Sheet sheet, SpriteScene scene)
    {
      _sheet = sheet;
      _scene = scene;
    }
    internal override void Redo()
    {
      _sheet.AddScene(_scene);
    }
    internal override void Undo()
    {
      _sheet.RemoveScene(_scene);
    }
  }
  class RemoveSpriteSceneCommand : ReversedCommand
  {
    public RemoveSpriteSceneCommand(Sheet sheet, SpriteScene scene) : base (new AddSpriteSceneCommand(sheet, scene)) {}
  }
  class SceneSpriteListTransformModifyCommand : Command
  {
    internal readonly List<ISceneSprite> _sprites;
    List<Transform> _last;
    List<Transform> _start;

    public SceneSpriteListTransformModifyCommand(List<ISceneSprite> sprites, List<Transform> start)
    {
      _sprites = sprites.Copy();
      _last = new List<Transform>();
      foreach (var s in sprites)
      {
        _last.Add(s.Transform.Duplicate());
      }
      _start = start.CloneItems(); 
    }

    internal override void Redo()
    {
      for (int i = 0; i < _sprites.Count(); i++)
      {
        _sprites[i].Transform = _last[i].Duplicate();
      }
    }
    internal override void Undo()
    {
      for (int i = 0; i < _sprites.Count(); i++)
      {
        _sprites[i].Transform = _start[i].Duplicate();
      }
    }
  }
  class RenameSpriteSceneCommand : Command
  {
    SpriteScene _scene;
    string _last;
    string _start;

    public RenameSpriteSceneCommand(SpriteScene scene, string start)
    {
      _scene = scene;
      _start = start;
      _last = scene.Name;
    }
    internal override void Redo()
    {
      _scene.Name = _last;
    }
    internal override void Undo()
    {
      _scene.Name = _start;
    }
  }
  // There SHOULD be no need to worry about parts that are removed later in the process
  // because, before this command is undone, the removed part will be undone first (as you cannot jump in the command history) and the whole state will 
  // be exactly the same before this command is called
  class ModifyScenePartCommand : Command
  {
    SpriteScene _scene;
    string _name;
    ISceneSprite _last;
    ISceneSprite _start;

    public ModifyScenePartCommand(SpriteScene scene, ISceneSprite sprite, ISceneSprite start)
    {
      _scene = scene;
      _name = sprite.Name;
      _start = start.Copy();
      _last = sprite.Copy();
    }
    internal override void Redo()
    {
      _scene.ReplaceSprite(_name, _last.Copy());
      _name = _last.Name;
    }
    internal override void Undo()
    {
      _scene.ReplaceSprite(_name, _start.Copy());
      _name = _start.Name;
    }
  }
  class AddScenePartCommand : Command
  {
    internal readonly SpriteScene _scene;
    ISceneSprite _sprite;

    public AddScenePartCommand(SpriteScene scene, ISceneSprite sprite)
    {
      _sprite = sprite;
      _scene = scene;
    }
    internal override void Redo()
    {
      _scene.AddSprite(_sprite);
    }
    internal override void Undo()
    {
      _scene.RemoveSprite(_sprite.Name);
    }
  }
  class RemoveScenePartCommand : ReversedCommand
  {
    public RemoveScenePartCommand(SpriteScene scene, ISceneSprite sprite) : base(new AddScenePartCommand(scene, sprite)) {}
  }

}
