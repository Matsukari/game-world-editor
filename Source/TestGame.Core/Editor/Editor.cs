using Nez;

namespace Raven
{
	public class Editor : Nez.Entity
	{
    public EditorSettings Settings = new EditorSettings();

    public Selection Selection;
    public Serializer Serializer;
    public ShapeAnnotator ShapeAnnotator;
    
    public ImGuiWinManager WindowManager;
    public ContentManager ContentManager;

    public static PrimitiveBatch PrimitiveBatch = new PrimitiveBatch();

    void OnCloseContent()
    {
      // Store last state
      ContentManager.GetContent().Data.Zoom = Scene.Camera.RawZoom;
      ContentManager.GetContent().Data.Position = Scene.Camera.Position;

      if (ContentManager.View.ImGuiHandler != null) 
        WindowManager.Renderables.Remove(ContentManager.View.ImGuiHandler);

      if (ContentManager.View.InputHandler != null)
        Core.GetGlobalManager<InputManager>().InputHandlers.Remove(ContentManager.View.InputHandler);
    }
    void OnOpenContent()
    {
      // Restore last state
      Scene.Camera.RawZoom = ContentManager.GetContent().Data.Zoom;
      Scene.Camera.Position = ContentManager.GetContent().Data.Position;

      if (ContentManager.View.ImGuiHandler != null) 
        WindowManager.Renderables.Add(ContentManager.View.ImGuiHandler);

      if (ContentManager.View.InputHandler != null)
        Core.GetGlobalManager<InputManager>().InputHandlers.Add(ContentManager.View.InputHandler);

    }
    void OnAddContent()
    {
      ContentManager.View.Initialize(this);
    }
    public override void OnAddedToScene()
    {
      Scene.Camera.Position = -Screen.Center / 2;

      ContentManager = new ContentManager();
      ContentManager.OnBeforeSwitch += OnCloseContent;
      ContentManager.OnAfterSwitch += OnOpenContent;
      ContentManager.OnAddContent += OnAddContent;

      Selection = AddComponent(new Selection());
      Serializer = new Serializer(ContentManager);
      ShapeAnnotator = new ShapeAnnotator();

      WindowManager = new ImGuiWinManager();
      WindowManager.Windows.Add(new Settings(Settings));
      WindowManager.Renderables.Add(new StatusBar(this));
      WindowManager.Renderables.Add(new Menubar(this));

      WindowManager.GetWindow<Settings>().OnSaveSettings += () => Serializer.SaveSettings();

      Serializer.LoadStartup();

      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent()); 
      AddComponent(new SelectionRenderer(Selection, Settings.Colors));

      var input = Core.GetGlobalManager<InputManager>();
      input.RegisterInputHandler(ShapeAnnotator);
      input.RegisterInputHandler(Selection);

      Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().RegisterDrawCommand(WindowManager.Render);
    }
    public override void Update()
    {
      base.Update();
      Scene.ClearColor = Settings.Colors.Background.ToColor();
      Settings.IsEditorBusy = Selection.HasBegun() || ShapeAnnotator.IsAnnotating;
      ContentManager.Update();
    }
    public override void OnRemovedFromScene()
    {
      Serializer.SaveSettings();
      PrimitiveBatch.Dispose();
    }
        
	}
}

