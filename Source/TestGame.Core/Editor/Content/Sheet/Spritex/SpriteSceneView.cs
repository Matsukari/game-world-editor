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
      _animationEditor.Initialize(editor, content);
      _animationEditor.OnClose += () => Selection.End();
    }
    // Go to canvas and close spritesheet view
    public void Edit(SpriteScene spriteScene)
    {
      Clean();
      IsEditing = true;

      // Prepare
      _sceneInspector = new SpriteSceneInspector(spriteScene);
      _sceneInspector.OnOpenAnimation += (scene, anim) => _animationEditor.Open(scene, anim);
      _sceneInspector.OnDelPart += part => Selection.End();
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
    void ReSelect(SourcedSprite part)
    {
      if (Selection.HasBegun()) Selection.Begin(part.SceneBounds, part);  
      _initialScale = part.Transform.Scale;
      _initialPos = part.Transform.Position;
      _initialRot = part.Transform.Rotation;
    }
    public void Render(Batcher batcher, Camera camera)
    {
      if (!IsEditing) return;
      if (Nez.Input.IsKeyReleased(Keys.Escape)) UnEdit(); 

      Guidelines.OriginLinesRenderable.Render(batcher, camera, 
          Settings.Colors.OriginLineX.ToColor(), 
          Settings.Colors.OriginLineY.ToColor());

      _renderer.Render(batcher, camera);

      foreach (var part in _sceneInspector.SpriteScene.Parts)
      {
        batcher.DrawCircle(part.PlainBounds.AddTransform(part.SpriteScene.Transform).Location, 4f/camera.RawZoom, Settings.Colors.OriginPoint.ToColor());

        if (part.SceneBounds.Contains(camera.MouseToWorldPoint()))
          batcher.DrawHollowRect(part.SceneBounds, Settings.Colors.SpriteBoundsOutline.ToColor(), 
              part.Transform.Rotation + part.SpriteScene.Transform.Rotation, Vector2.Zero, 1f/camera.RawZoom);

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
      if (Selection.Capture is SourcedSprite selPart)
      {
        selPart.Transform.Scale = _initialScale + 
          (Selection.ContentBounds.Size - Selection.InitialBounds.Size) / (selPart.SourceSprite.Region.Size.ToVector2());
        selPart.Transform.Position = _initialPos + (Selection.ContentBounds.Location - Selection.InitialBounds.Location) +
          ( selPart.Origin *
          ((Selection.ContentBounds.Size - Selection.InitialBounds.Size) / (selPart.SourceSprite.Region.Size.ToVector2())) );
      }
      if (Mover.Capture is SourcedSprite p)
      {
        p.Transform.Position = _initialPos + ((Mover.Position - p.Origin) - (Mover.InitialPosition - p.Origin));        
      }
      if (Rotator.Capture is SourcedSprite p2)
      {
        p2.Transform.Rotation = _initialRot + (Rotator.Angle  - Rotator.InitialAngle);
      }
    }  
    Vector2 _initialScale = new Vector2();
    Vector2 _initialPos = new Vector2();
    float _initialRot = 0;

    public bool HandleInput(InputManager input)
    {
      if (!IsEditing) return false;

      var mouse = Camera.MouseToWorldPoint();
      var part = _renderer.GetPartAtWorld(mouse);

      if (part != null)
      {
        if (Operator == EditorOperator.MoveOnly) Mover.Show(part.Bounds.Center);

        if (Nez.Input.RightMouseButtonPressed)
        {
          _sceneInspector._isOpenComponentOptionPopup = true;
          _sceneInspector._compOnOptions = part;
          return true;
        }
        if (Nez.Input.LeftMouseButtonPressed)
        {
          _initialScale = part.Transform.Scale;
          _initialPos = part.Transform.Position;
          _initialRot = part.Transform.Rotation;

          if (Operator == EditorOperator.Select) Selection.Begin(part.SceneBounds, part); 
          else if (Operator == EditorOperator.MoveOnly) Mover.TryBegin(part.Bounds.Center, input.Camera, part); 
          else if (Operator == EditorOperator.Rotator) Rotator.Begin(part.Bounds.Location, input.Camera, part); 
        }
      }
      else if (Operator == EditorOperator.MoveOnly && Mover.Collides(Camera) == Guidelines.MovableOriginLines.AxisType.None) Mover.Hide();

      if (Nez.Input.RightMouseButtonPressed)
      {
        _sceneInspector._isOpenSceneOnSpritePopup = true; 
        _sceneInspector._posOnOpenCanvas = Camera.MouseToWorldPoint();
        return true;
      }
      return false;
    }
  }
}
