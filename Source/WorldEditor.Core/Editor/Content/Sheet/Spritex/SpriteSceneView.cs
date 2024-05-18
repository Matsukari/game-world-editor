using Nez;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace Raven
{

  public class TransformList : List<Transform>
  {
    
  }
  // <summary>
  // Handles state changes between sheet and spriteScene 
  // </summary>
  public class SpriteSceneView : EditorInterface, IInputHandler
  {
    public SpriteSceneInspector SceneInspector { get => _sceneInspector; }
    public SpriteSceneInspector LastSprite { get => _sceneInspector; }
    public SpriteSceneRenderer Renderer { get => _renderer; }
    public SpriteScene SpriteScene { get => _sceneInspector.SpriteScene; }
    public AnimationEditor AnimationEditor { get => _animationEditor; }
    public Widget.AnnotatorPane AnnotatorPane;

    public AnimationEditor _animationEditor = new AnimationEditor();
    readonly SheetView _sheetView;
    public bool IsEditing;
    SpriteSceneInspector _sceneInspector;
    SpriteSceneRenderer _renderer;
    CommandManager _commandManager;

    public event Action OnEdit;
    public event Action OnUnEdit;

    public SpriteSceneView(SheetView view)
    {
      _sheetView = view;
      IsEditing = false;
    }
    public override void Initialize(Editor editor, EditorContent content)
    {
      base.Initialize(editor, content);
      AnnotatorPane = new Widget.AnnotatorPane(editor.Settings.Colors);
      _animationEditor.Initialize(editor, content);
      _animationEditor.OnClose += () => Selection.End();
      Selection.OnMoveStart += SelectionDragStart;
      Selection.OnMoveEnd += SelectionDragEnd;
      Selection.OnScaleStart += SelectionDragStart;
      Selection.OnScaleEnd += SelectionDragEnd;
      _commandManager = new CommandManager();
    }
    // Go to canvas and close spritesheet view
    public void Edit(SpriteScene spriteScene)
    {
      Clean();
      IsEditing = true;

      ContentData.Position = Camera.Position;
      ContentData.Zoom = Camera.RawZoom;

      // Prepare
      _sceneInspector = new SpriteSceneInspector(spriteScene);
      _sceneInspector.RenderAttachments += imgui => RenderAnnotations(SpriteScene);
      _sceneInspector.OnOpenAnimation += (scene, anim) => 
      {
        if (anim is AnimatedSprite animatedSprite)
        {
          Console.WriteLine("Openning AnimatedSprite");
          _sheetView.SpriteAnimationEditor.Open(animatedSprite);
        }
        else 
          _animationEditor.Open(scene, anim);
      };
      _sceneInspector.OnDelPart += part => Selection.End();
      _sceneInspector.OnEmbedShape += part => AnnotatorPane.Edit(part, part);
      _sceneInspector.OnModifiedPart += ReSelect; 
      _renderer = new SpriteSceneRenderer(spriteScene);
      _renderer.Entity = Entity;
      ContentData.PropertiedContext = spriteScene;

      Core.GetGlobalManager<CommandManagerHead>().Current = _commandManager;
      if (OnEdit != null) OnEdit();
    }

    // back to spritesheet view
    public void UnEdit()
    {
      if (!IsEditing) return;

      Clean();
      if (OnUnEdit != null) OnUnEdit();

      // Sotre last state
      _sceneInspector.GuiPosition = Camera.Position;
      _sceneInspector.GuiZoom = Camera.RawZoom;

      // Enter sheet vew
      Camera.RawZoom = ContentData.Zoom;
      Camera.Position = ContentData.Position;
    }
    void Clean()
    {
      IsEditing = false;
      Selection.End();
    }
    void ReSelect(ISceneSprite part)
    {
      if (Selection.Capture is List<ISceneSprite> sels && sels.Count() != 1) return;
      Selection.Begin(part.SceneBounds, part);  
      _initialTransform.Clear();
      _initialTransform.Add(part.Transform.Duplicate());
    }
    void SelectionDragStart()
    {
      _startTransform.Clear();
      for (int i = 0; i < _multiSels.Count(); i++)
      {
        _startTransform.Add(_multiSels[i].Transform.Duplicate());
      }
    }
    void SelectionDragEnd()
    {
      if (_multiSels.Count() != _startTransform.Count()) return;
      var command = new SceneSpriteListTransformModifyCommand(_multiSels, _startTransform);
      _commandManager.Record(command, ()=>Selection.Re(command._sprites.EnclosedBounds(), command._sprites, ()=>StartTransform(command._sprites)));
      _startTransform.Clear();
    }
    public void Render(Batcher batcher, Camera camera)
    {
      if (!IsEditing) return;
      if (Nez.Input.IsKeyReleased(Keys.Escape)) UnEdit(); 

      Guidelines.OriginLinesRenderable.Render(batcher, camera, 
          Settings.Colors.OriginLineX.ToColor(), 
          Settings.Colors.OriginLineY.ToColor());

      _renderer.Render(batcher, camera);


      foreach (var part in _multiSels)
      {
        batcher.DrawRectOutline(camera, part.SceneBounds, Settings.Colors.PickSelectedOutline.ToColor());
      }
      batcher.DrawRect(_multiSelection, Settings.Colors.SelectionFill.ToColor());
      batcher.DrawRectOutline(camera, _multiSels.EnclosedBounds(), Settings.Colors.SelectionOutline.ToColor());
      batcher.DrawRectOutline(camera, _multiSelection, Settings.Colors.SelectionOutline.ToColor());

      foreach (var part in _sceneInspector.SpriteScene.Parts)
      {
        var origin = part.PlainBounds.AddTransform(part.SpriteScene.Transform);
        origin.Size = new Vector2(4);
        batcher.DrawRectOutline(camera, origin, Settings.Colors.OriginPoint.ToColor());

        batcher.DrawString(
            Graphics.Instance.BitmapFont, 
            part.Name,
            _renderer.GetPartWorldBounds(part).BottomLeft(),
            color: Settings.Colors.ScenePartName.ToColor(), 
            rotation: 0f, 
            origin: Vector2.Zero, 
            scale: Math.Clamp(1f/camera.RawZoom, 1f, 10f), 
            effects: Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 
            layerDepth: 0f);
      }
      if (Selection.Capture is List<ISceneSprite> sels)
      {
        for (int i = 0; i < sels.Count(); i++)
        {
          var contentScale = (Selection.ContentBounds.Size - Selection.InitialBounds.Size) / Selection.InitialBounds.Size;
          sels[i].Transform.Scale = _initialTransform[i].Scale + contentScale * _initialTransform[i].Scale;
          sels[i].Transform.Position = _initialTransform[i].Position 
            + (Selection.ContentBounds.Location - Selection.InitialBounds.Location) 
            + (sels[i].Origin * contentScale)
            + (contentScale * (_initialTransform[i].Position - Selection.InitialBounds.Location));

        }
      }
      if (Mover.Capture is List<ISceneSprite> moving)
      {
        for (int i = 0; i < moving.Count(); i++)
        {
          moving[i].Transform.Position = _initialTransform[i].Position + ((Mover.Position - moving[i].Origin) - (Mover.InitialPosition - moving[i].Origin));        
        }
      }
      if (Rotator.Capture is List<ISceneSprite> rotating)
      {
        for (int i = 0; i < rotating.Count(); i++)
        {
          rotating[i].Transform.Rotation = _initialTransform[i].Rotation + (Rotator.Angle  - Rotator.InitialAngle);
        }
      }
    }  
    List<Transform> _startTransform = new List<Transform>();
    List<Transform> _initialTransform = new List<Transform>();
    List<ISceneSprite> _multiSels = new List<ISceneSprite>();
    RectangleF _multiSelection = new RectangleF();

    void IInputHandler.OnGuiIntercept(Raven.InputManager input)
    {
      Selection.End();
    }

    bool IInputHandler.OnHandleInput(Raven.InputManager input)
    {
      if (!IsEditing) return false;

      var mouse = Camera.MouseToWorldPoint();

      if (_multiSels.Count > 0)
      {
        if (_multiSels.EnclosedBounds().Contains(mouse))
        {
          if (Operator == EditorOperator.MoveOnly) Mover.Show(_multiSels.EnclosedBounds().Center);
          if (Operator != EditorOperator.Select) Selection.End();
        }

        if (Nez.Input.RightMouseButtonPressed)
        {
          _sceneInspector._isOpenComponentOptionPopup = true;
          _sceneInspector._compOnOptions = _multiSels.Last();
          return true;
        }
      }
      if (Mover.Collides(Camera) == Guidelines.MovableOriginLines.AxisType.None) Mover.Hide();

      if (Nez.Input.RightMouseButtonPressed)
      {
        _sceneInspector._isOpenSceneOnSpritePopup = true; 
        _sceneInspector._posOnOpenCanvas = Camera.MouseToWorldPoint();
        return true;
      }


      if (Nez.Input.LeftMouseButtonPressed)
      {

        if (_multiSels.Count > 0 && Operator == EditorOperator.MoveOnly) 
        {
          Mover.TryBegin(_multiSels.EnclosedBounds().Center, Camera, _multiSels);
        }
        else if (_multiSels.Count > 0 && Operator == EditorOperator.Rotator) 
        {
          if (_multiSels.Count() == 1) Rotator.Begin(_multiSels.Last().Bounds.Location, Camera, _multiSels);
          else Rotator.Begin(_multiSels.EnclosedBounds().Center, Camera, _multiSels);
        }
        else if (!_multiSelection.Contains(mouse))
        {
          // Console.WriteLine("Started");
          _multiSels.Clear();
          _multiSelection = new RectangleF();
        }
      }
      if (Nez.Input.LeftMouseButtonDown && !Rotator.IsVisible && !Mover.IsMoving && !Selection.HasBegun())
      {
        _multiSelection = RectangleF.FromMinMax(Camera.ScreenToWorldPoint(input.MouseDragStart), mouse).AlwaysPositive();

        if (_multiSelection.Size.X >= 10 || _multiSelection.Size.Y >= 10)
        {
          foreach (var comp in SpriteScene.Parts)
          {
            if (_multiSelection.Intersects(comp.SceneBounds))
            {
              if (_multiSels.Find(item => item.Name == comp.Name) == null)
                _multiSels.AddIfNotPresent(comp);
            }
            else 
            {
              _multiSels.RemoveAll(item => item.Name == comp.Name);
            }
          }
        }
        else
        {
          _multiSels.Clear();
          var scene = SpriteScene.Parts.FindLast(item => item.SceneBounds.Intersects(_multiSelection));
          if (scene != null)
          {
            _multiSels.Add(scene);
          }
        }
      }
      if (input.IsDragLast)
      {
        _multiSelection = new RectangleF();
        if (_multiSels.Count <= 0) 
        {
          return false;
        }
        StartTransform(_multiSels);

        if (Operator == EditorOperator.Select) Selection.Begin(_multiSels.EnclosedBounds(), _multiSels);
      }


      return false;
    }
    void StartTransform(List<ISceneSprite> sels)
    {
      _initialTransform.Clear();
      foreach (var i in sels)
        _initialTransform.Add(i.Transform.Duplicate());
    }
  }
}
