using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{
  internal static class EventHelpers
  {
    internal static void Raise<TEventArgs>(object sender, EventHandler<TEventArgs> handler, TEventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }
    internal static void Raise(object sender, EventHandler handler, EventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }
  }

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

    ContentView _nextView;
    IPropertied _nextContent;
    public Editor(ContentView view, IPropertied content)
    {
      _nextView = view;
      _nextContent = content;
    }
    public Editor()
    {
    }

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

      WindowManager = new ImGuiWinManager();
      ContentManager = new ContentManager(Settings, WindowManager);
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

      WindowManager.AddImmediate(new Settings(Settings));
      WindowManager.AddImmediate(new StatusBar(this));
      WindowManager.AddImmediate(new Menubar(this));
      WindowManager.AddImmediate(ShapeAnnotator);
      WindowManager.AddImmediate(new History());
      WindowManager.AddImmediate(new WindowHolder("Content"), "main");
      WindowManager.AddImmediate(new WindowHolder("Inspector"), "sub");

      WindowManager.GetRenderable<Settings>().OnSaveSettings += () => Serializer.SaveSettings();
      Serializer.LoadStartup();

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



      if (_nextView != null && _nextContent != null)
      {
        ContentManager.AddTab(_nextView, _nextContent);
        Settings.LastFile = ContentManager._tabs.Count - 1;
      }
      if (Settings.LastFiles.Count() == 0 && ContentManager._tabs.Count() == 0)
      {
        throw new Exception("Must provide a default content if there are no recent files");
      }

      ContentManager.Switch(0, true);

      Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().RegisterDrawCommand(WindowManager.Render);
    }
    public override void Update()
    {
      base.Update();
      Scene.ClearColor = Settings.Colors.Background.ToColor();
      Settings.IsEditorBusy = Selection.HasBegun() || ShapeAnnotator.IsAnnotating;
      ContentManager.Update();
      UpdateSelections();

    
      if (!Settings.Graphics.FixedWindowSize) return;

      var main = WindowManager.GetRenderable<WindowHolder>("main");
      var sub = WindowManager.GetRenderable<WindowHolder>("sub");

      if (sub.Bounds.Size != Vector2.Zero)
      {
        sub.SchedWindowPos = new Vector2(Screen.Width-Settings.Graphics.InitialInspectorWindowWidth, 32);
        sub.SchedWindowSize.X = Settings.Graphics.InitialInspectorWindowWidth;        
        sub.SchedWindowSize.Y = WindowManager.GetRenderable<StatusBar>().Bounds.Y - WindowManager.GetRenderable<StatusBar>().Bounds.Size.Y;
      }

      if (main.Bounds.Size != Vector2.Zero)
      {
        main.SchedWindowPos = new Vector2(0f, 32);
        main.SchedWindowSize.X = Settings.Graphics.InitialContentWindowWidth;        
        main.SchedWindowSize.Y = WindowManager.GetRenderable<StatusBar>().Bounds.Y - WindowManager.GetRenderable<StatusBar>().Bounds.Size.Y;
      }

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

