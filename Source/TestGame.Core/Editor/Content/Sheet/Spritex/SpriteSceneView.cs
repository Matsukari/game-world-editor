using Nez;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace Raven
{
  // <summary>
  // Handles state changes between sheet and spriteScene 
  // </summary>
  public class SpriteSceneView : EditorInterface
  {
    public SpriteSceneInspector SceneInspector { get => _sceneInspector; }
    public SpriteSceneInspector LastSprite { get => _sceneInspector; }
    public SpriteSceneRenderer Renderer { get => _renderer; }
    public SpriteScene SpriteScene { get => _sceneInspector.SpriteScene; }
    public AnimationEditor AnimationEditor { get => _animationEditor; }
    public Widget.AnnotatorPane AnnotatorPane;

    public AnimationEditor _animationEditor = new AnimationEditor();
    readonly SheetView _sheetView;
    SpriteSceneInspector _sceneInspector;
    SpriteSceneRenderer _renderer;
    public bool IsEditing;

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
      _initialScale.Add(Vector2.Zero);
      _initialPos.Add(Vector2.Zero);
      _initialRot.Add(0);
    }
    // Go to canvas and close spritesheet view
    public void Edit(SpriteScene spriteScene)
    {
      Clean();
      IsEditing = true;

      // Prepare
      _sceneInspector = new SpriteSceneInspector(spriteScene);
      _sceneInspector.OnOpenAnimation += (scene, anim) => 
      {
        if (anim is AnimatedSprite)
        {
          Console.WriteLine("Openning AnimatedSprite");
          _sheetView.SpriteAnimationEditor.Open(anim);
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
      if (OnEdit != null) OnEdit();
    }

    // back to spritesheet view
    public void UnEdit()
    {
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
    }
    void ReSelect(ISceneSprite part)
    {
      if (Selection.HasBegun()) Selection.Begin(part.SceneBounds, part);  
      _initialScale[0] = part.Transform.Scale;
      _initialPos[0] = part.Transform.Position;
      _initialRot[0] = part.Transform.Rotation;
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
        batcher.DrawRectOutline(part.PlainBounds.AddTransform(part.SpriteScene.Transform), Settings.Colors.PickSelectedOutline.ToColor());
      }
      batcher.DrawRect(_multiSelection, Settings.Colors.SelectionFill.ToColor());
      batcher.DrawRectOutline(camera, _multiSelection, Settings.Colors.SelectionOutline.ToColor());

      foreach (var part in _sceneInspector.SpriteScene.Parts)
      {
        batcher.DrawCircle(part.PlainBounds.AddTransform(part.SpriteScene.Transform).Location, 4f/camera.RawZoom, Settings.Colors.OriginPoint.ToColor());

        // if (part.SceneBounds.Contains(camera.MouseToWorldPoint()))
        //   batcher.DrawHollowRect(part.SceneBounds, Settings.Colors.SpriteBoundsOutline.ToColor(), 
        //       part.Transform.Rotation + part.SpriteScene.Transform.Rotation, Vector2.Zero, 1f/camera.RawZoom);

        batcher.DrawString(
            Graphics.Instance.BitmapFont, 
            part.Name,
            _renderer.GetPartWorldBounds(part).BottomLeft(),
            color: Color.DarkGray, 
            rotation: 0f, 
            origin: Vector2.Zero, 
            scale: Math.Clamp(1f/camera.RawZoom, 1f, 10f), 
            effects: Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 
            layerDepth: 0f);
      }
      if (Selection.Capture is List<ISceneSprite> sels)
      {
        int i = 0;
        // Console.WriteLine("Num of sels: " + sels.Count);
        foreach (var sel in sels)
        {
          i++;
          var contentScale = (Selection.ContentBounds.Size - Selection.InitialBounds.Size) / Selection.InitialBounds.Size;
          sel.Transform.Scale = _initialScale[i] + contentScale * _initialScale[i];
          sel.Transform.Position = _initialPos[i] + (Selection.ContentBounds.Location - Selection.InitialBounds.Location) + (sel.Origin * contentScale);
          sel.Transform.Position += (contentScale * (_initialPos[i] - Selection.InitialBounds.Location));

        }
      }
      if (Mover.Capture is ISceneSprite p)
      {
        p.Transform.Position = _initialPos[0] + ((Mover.Position - p.Origin) - (Mover.InitialPosition - p.Origin));        
      }
      if (Rotator.Capture is ISceneSprite p2)
      {
        p2.Transform.Rotation = _initialRot[0] + (Rotator.Angle  - Rotator.InitialAngle);
      }
    }  
    List<Vector2> _initialScale = new List<Vector2>();
    List<Vector2> _initialPos = new List<Vector2>();
    List<float> _initialRot = new List<float>();
    RectangleF _multiSelection = new RectangleF();
    List<ISceneSprite> _multiSels = new List<ISceneSprite>();

    public bool HandleInput(InputManager input)
    {
      if (!IsEditing) return false;

      var mouse = Camera.MouseToWorldPoint();

      if (_multiSels.Count > 0)
      {
        if (Operator == EditorOperator.MoveOnly) Mover.Show(_multiSels.EnclosedBounds().Center);

        if (Nez.Input.RightMouseButtonPressed)
        {
          _sceneInspector._isOpenComponentOptionPopup = true;
          _sceneInspector._compOnOptions = _multiSels.Last();
          return true;
        }
      }
      else if (Operator == EditorOperator.MoveOnly && Mover.Collides(Camera) == Guidelines.MovableOriginLines.AxisType.None) Mover.Hide();

      if (Nez.Input.RightMouseButtonPressed)
      {
        _sceneInspector._isOpenSceneOnSpritePopup = true; 
        _sceneInspector._posOnOpenCanvas = Camera.MouseToWorldPoint();
        return true;
      }


      if (Nez.Input.LeftMouseButtonPressed && !_multiSelection.Contains(mouse))
      {
        // Console.WriteLine("Started");
        _multiSels.Clear();
        _multiSelection = new RectangleF();
      }
      if (Nez.Input.LeftMouseButtonDown)
      {
        _multiSelection = RectangleF.FromMinMax(Camera.ScreenToWorldPoint(input.MouseDragStart), mouse).AlwaysPositive();

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
      if (input.IsDragLast)
      {
        _multiSelection = new RectangleF();
        if (_multiSels.Count <= 0) 
        {
          // Console.WriteLine("Ends");
          // Selection.End();
          return false;
        }
        // _multiSelection = new RectangleF();
        if (_initialPos.Count() > 1)
        {
          _initialPos.RemoveRange(1, _initialPos.Count()-1);
          _initialScale.RemoveRange(1, _initialScale.Count()-1);
          _initialRot.RemoveRange(1, _initialRot.Count()-1);
        }
        foreach (var p in _multiSels)
        {
         
          _initialPos.Add(p.Transform.Position);
          _initialScale.Add(p.Transform.Scale);
          _initialRot.Add(p.Transform.Rotation);
        }
        Selection.Begin(_multiSels.EnclosedBounds(), _multiSels);
      }

      return false;
    }
  }
}
