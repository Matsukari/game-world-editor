using Nez;

namespace Raven
{
  public enum EditorOperator { Select, MoveOnly, HandPanner, Scaler, Rotator }

	public class Editor : Nez.Entity
	{
    public EditorSettings Settings = new EditorSettings();

    public EditorOperator Operator;
    public Guidelines.MovableOriginLines Mover;
    public Rotator Rotator;
    public Selection Selection;
    public Serializer Serializer;
    public ShapeAnnotator ShapeAnnotator;
    
    public ImGuiWinManager WindowManager;
    public ContentManager ContentManager;

    public static PrimitiveBatch PrimitiveBatch = new PrimitiveBatch();

    void OnCloseContent(EditorContent content, ContentView view)
    {
      // Store last state
      content.Data.Zoom = Scene.Camera.RawZoom;
      content.Data.Position = Scene.Camera.Position;

      WindowManager.RemoveRenderable(view.ImGuiHandler);

      if (view.InputHandler != null)
        Core.GetGlobalManager<InputManager>().InputHandlers.Remove(view.InputHandler);
    }
    void OnOpenContent(EditorContent content, ContentView view)
    {
      // Restore last state
      Scene.Camera.RawZoom = content.Data.Zoom;
      Scene.Camera.Position = content.Data.Position;

      WindowManager.AddRenderable(view.ImGuiHandler);

      if (view.InputHandler != null)
        Core.GetGlobalManager<InputManager>().InputHandlers.AddIfNotPresent(view.InputHandler);

    }
    void OnAddContent(EditorContent content, ContentView view)
    {
      view.Initialize(this, content);
    }
    void ShapePostProcessor(ShapeModel shape, IPropertied context)
    {
      if (context is Level level) shape.Bounds = shape.Bounds.AddPosition(-level.Bounds.Location);
    }
    public override void OnAddedToScene()
    {
      Scene.Camera.Position = -Screen.Center / 2;

      Operator = EditorOperator.Select;

      ContentManager = new ContentManager(Settings);
      ContentManager.OnCloseContent += OnCloseContent;
      ContentManager.OnOpenContent += OnOpenContent;
      ContentManager.OnAddContent += OnAddContent;

      Selection = AddComponent(new Selection());
      Serializer = new Serializer(ContentManager);
      ShapeAnnotator = new ShapeAnnotator(Settings);
      ShapeAnnotator.PostProcess += ShapePostProcessor;
      Mover = AddComponent(new Guidelines.MovableOriginLines());
      Mover.RenderLayer = -1;
      Rotator = AddComponent(new Rotator());
      Rotator.RenderLayer = -1;

      WindowManager = new ImGuiWinManager();
      WindowManager.Renderables.Add(new Settings(Settings));
      WindowManager.Renderables.Add(new StatusBar(this));
      WindowManager.Renderables.Add(new Menubar(this));
      WindowManager.Renderables.Add(ShapeAnnotator);

      WindowManager.GetRenderable<Settings>().OnSaveSettings += () => Serializer.SaveSettings();

      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent()); 
      AddComponent(new ContentRenderer(ContentManager));
      AddComponent(new SelectionRenderer(Selection, Settings.Colors));

      var input = Core.GetGlobalManager<InputManager>();
      input.RegisterInputHandler(ShapeAnnotator);
      input.RegisterInputHandler(Selection);
      input.RegisterInputHandler(Mover);
      input.RegisterInputHandler(Rotator);
      input.RegisterInputHandler(GetComponent<SelectionRenderer>());

      Serializer.LoadStartup();

      var sheet = new Sheet("/home/ark/Documents/game/projects/WorldEditor/Assets/Raw/Unprocessed/big_forest.png");
      ContentManager.AddTab(new SheetView(), sheet);
      var world = new World();
      world.AddSheet(sheet);
      ContentManager.AddTab(new WorldView(), world);

      ContentManager.Switch(1, true);

      Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().RegisterDrawCommand(WindowManager.Render);
    }
    public override void Update()
    {
      base.Update();
      Scene.ClearColor = Settings.Colors.Background.ToColor();
      Settings.IsEditorBusy = Selection.HasBegun() || ShapeAnnotator.IsAnnotating;
      ContentManager.Update();
      UpdateSelections();
    }
    public void UpdateSelections()
    {
      if (Selection.Capture is ShapeModel shapeSel)
        shapeSel.Bounds = Selection.ContentBounds;
    }
    public override void OnRemovedFromScene()
    {
      Serializer.SaveSettings();
      PrimitiveBatch.Dispose();
    }
        
	}
}

