using Raven.Sheet;
using Nez;

namespace Raven
{
  public enum EditingState { SelectedSprite, AnnotateShape, Default, Modal };

  // Implement along with EditorComponent to render imgui content
  public interface IImGuiRenderable 
  {
    public void Render(Editor editor);
  }
  public class ImGuiWinManager
  {
    public List<Widget.Window> Windows = new List<Widget.Window>();
    public List<IImGuiRenderable> Renderables = new List<IImGuiRenderable>();
    public Widget.FilePicker FilePicker = new Widget.FilePicker();
    public Widget.NameModal NameModal = new Widget.NameModal();

    public T GetWindow<T>() 
    {
      foreach (var window in Windows) 
      {
        if (window is T windowType) return windowType;
      }
      throw new Exception();
    }
    public void AddIfNotNull(IImGuiRenderable guiRenderable)
    {
      if (guiRenderable != null) Renderables.Add(guiRenderable);
    }
    public void Render(Editor editor)
    {
      foreach (var window in Windows) window.Render(editor);
      foreach (var renderable in Renderables) renderable.Render(editor);
      NameModal.Draw();
      FilePicker.Draw();
    }
  }
  public class ContentManager 
  {
    public bool HasContent { get => _tabs.Count() > 0; }
    public ContentView View { get => _views[_currentTab]; }
    public IPropertied Content { get => _tabs[_currentTab].Content; }
    public EditorContentData ContentData { get => _tabs[_currentTab].Data; }
    public int CurrentIndex { get => _currentTab; }

    public EditorSettings Settings;
    public event Action OnBeforeSwitch;
    public event Action OnAfterSwitch;
    public event Action OnAddContent;

    SheetView sheetView;
    WorldView worldView;

    // Either world or sheet; these are the objects that can be switch in and out
    internal List<EditorContent> _tabs = new List<EditorContent>();
    internal List<ContentView> _views = new List<ContentView>();
    int _currentTab = 0;

    public void Render(Batcher batcher, Camera camera) => View.Render(batcher, camera, Settings);
    public void Update()
    {
      if (HasContent)
      {
        Settings.LastFiles[_currentTab].Filename = Content.Name;
        ContentData.Filename = Content.Name;
      }
    }
    public void Switch(int index) 
    {
      Console.WriteLine($"Switched to {index}");

      OnBeforeSwitch();
      if (HasContent) 
        View.OnContentClose();
      _currentTab = Math.Clamp(index, 0, _tabs.Count()-1);
      View.OnContentOpen(Content);
      OnAfterSwitch();

      Settings.LastFile = index;
    }
    public EditorContent GetContent() => _tabs[_currentTab];

    public void AddTab(ContentView contentView, IPropertied content, bool isSwitch=true)
    {
      Console.WriteLine("Adding content on tabs");

      var contentData = new EditorContentData(content.Name, content.GetType().Name);
      // This file already exist within the tab files
      if (Settings.LastFiles.Find((file)=>file.Filename == content.Name) != null)
      {
        Console.WriteLine("Cannot add Content. Already exist.");
        return;
      }
      if (!contentView.CanDealWithType(content))
        throw new Exception();

      Settings.LastFiles.Add(contentData);

      _tabs.Add(new EditorContent(content, contentData));
      _views.Add(contentView);

      OnAddContent();

      // First tab in the list yet
      if (_tabs.Count() == 1 || isSwitch) Switch(_tabs.Count()-1);
    }
  }


	public class Editor : Nez.Entity
	{
    public EditorSettings Settings = new EditorSettings();

    public Selection Selection;
    public Serializer Serializer;
    public ShapeAnnotator ShapeAnnotator;
    
    Settings _settingsWindow;

    public ImGuiWinManager WindowManager;
    public ContentManager ContentManager;

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
    }
    public override void OnAddedToScene()
    {
      Scene.Camera.Position = -Screen.Center / 2;

      ContentManager.OnBeforeSwitch += OnCloseContent;
      ContentManager.OnAfterSwitch += OnOpenContent;

      Selection = AddComponent(new Selection());
      Serializer = new Serializer(ContentManager);
      ShapeAnnotator = new ShapeAnnotator();

      WindowManager = new ImGuiWinManager();
      WindowManager.Windows.Add(new Settings(Settings));

      Serializer.LoadStartup();

      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent()); 
      AddComponent(new SelectionRenderer(Selection, Settings.Colors));

      var input = Core.GetGlobalManager<InputManager>();
      input.RegisterInputHandler(ShapeAnnotator);
      input.RegisterInputHandler(Selection);

          new SheetSelector(),
          new Menubar(),
          new StatusBar(),
          new SpriteSceneView(),
          new AnimationEditor(),
          new WorldEditor());

      Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().RegisterDrawCommand(RenderImGui);
    }
    void RenderImGui()
    {
      WindowManager.Render(this);
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
    }
        
	}
}

