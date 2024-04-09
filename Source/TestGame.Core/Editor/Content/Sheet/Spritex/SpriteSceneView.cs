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
        selPart.Transform.Position = _initialPos + ((Selection.ContentBounds.Location - selPart.Origin) - (Selection.InitialBounds.Location - selPart.Origin));
      }
    }  
    Vector2 _initialScale = new Vector2();
    Vector2 _initialPos = new Vector2();
    public bool HandleInput(InputManager input)
    {
      if (!IsEditing) return false;

      var mouse = Camera.MouseToWorldPoint();
      var part = _renderer.GetPartAtWorld(mouse);

      if (part != null)
      {
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
          Selection.Begin(part.Bounds, part);
          return true;
        }
      }
      else if (Nez.Input.RightMouseButtonPressed)
      {
        _sceneInspector._isOpenSceneOnSpritePopup = true; 
        _sceneInspector._posOnOpenCanvas = Camera.MouseToWorldPoint();
        return true;
      }
      return false;
    }
  }
}
