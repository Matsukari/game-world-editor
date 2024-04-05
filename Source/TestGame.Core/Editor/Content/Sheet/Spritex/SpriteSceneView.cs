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
    public SheetInspector Inspector { get => _sheetView.Inspector; }
    public SpriteSceneInspector SceneInspector { get => _sceneInspector; }
    public SpriteSceneInspector LastSprite { get => _sceneInspector; }
    public SpriteSceneRenderer Renderer { get => _renderer; }
    public SpriteScene SpriteScene { get => _sceneInspector.SpriteScene; }
    public AnimationEditor _animationEditor = new AnimationEditor();
    readonly SheetView _sheetView;
    SpriteSceneInspector _sceneInspector;
    SpriteSceneRenderer _renderer;
    public bool IsEditing;

    public SpriteSceneView(SheetView view)
    {
      _sheetView = view;
      IsEditing = false;
    }
    // Go to canvas and close spritesheet view
    public void Edit(SpriteScene spriteScene)
    {
      Clean();
      IsEditing = true;

      _sheetView.Inspector.SpritePicker.HandleSelectedSprite = new SpriteSceneSpritePicker(_sheetView.Inspector.SpritePicker).HandleSelectedSprite;

      // Prepare
      _sceneInspector = new SpriteSceneInspector(spriteScene);
      _sceneInspector.OnOpenAnimation += (scene, anim) => _animationEditor.Open(scene, anim);
      _renderer = new SpriteSceneRenderer(spriteScene);
      ContentData.PropertiedContext = spriteScene;
      Inspector.ShowPicker = true; 
    }

    // back to spritesheet view
    public void UnEdit()
    {
      Clean();
      Inspector.ShowPicker = false; 

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
          Inspector._settings.Colors.OriginLineX.ToColor(), 
          Inspector._settings.Colors.OriginLineY.ToColor());

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

    }  
    Vector2 _initialScale = new Vector2();
    public bool HandleInput()
    {
      if (!IsEditing) return false;

      var selectionRect = Selection;
      var mouse = Camera.MouseToWorldPoint();
      var part = _renderer.GetPartAtWorld(mouse);
      if (Nez.Input.RightMouseButtonPressed && part != null)
      {
        _sceneInspector._isOpenComponentOptionPopup = true;
        _sceneInspector._compOnOptions = part;
        return true;
      }
      if (Nez.Input.LeftMouseButtonPressed)
      {
        _initialScale = part.Transform.Scale;
        selectionRect.Begin(_renderer.GetPartWorldBounds(part), part);
        return true;
      }
      if (selectionRect.Capture is SourcedSprite selPart)
      {
        selPart.Transform.Scale = _initialScale + 
          (selectionRect.ContentBounds.Size - selectionRect.InitialBounds.Size) / (selPart.SourceSprite.Region.Size.ToVector2());
        selPart.Transform.Position = selectionRect.ContentBounds.Location + (selPart.Origin * selPart.Transform.Scale);
        return true;
      }
      return false;
    }
  }
}
