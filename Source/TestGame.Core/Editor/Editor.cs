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
    public void Render(Editor editor)
    {
      foreach (var window in Windows) window.Render(editor);
      NameModal.Draw();
      FilePicker.Draw();
    }
  }
  public class ContentManager 
  {
    public bool HasContent { get => _tabs.Count() > 0; }
    public ContentView View {
      get
      {
        if (GetContent().Content is Sheet) return sheetView;
        else if (GetContent().Content is World) return worldView;
        else throw new Exception();
      }
    }

    public EditorSettings Settings;
    public event Action OnBeforeSwitch;
    public event Action OnAfterSwitch;

    SheetView sheetView;
    WorldView worldView;

    // Either world or sheet; these are the objects that can be switch in and out
    internal List<EditorContent> _tabs = new List<EditorContent>();
    int _currentTab = 0;

    public void Render(Editor editor) => View.Render(editor);
    public void Render(Batcher batcher, Camera camera) => View.Render(batcher, camera);
    public void Update()
    {
      if (HasContent)
      {
        Settings.LastFiles[_currentTab].Filename = GetContent().Content.Name;
        GetContent().Data.Filename = GetContent().Content.Name;
    
        View.Update();
      }
    }
    public void Switch(int index) 
    {
      Console.WriteLine($"Switched to {index}");

      OnBeforeSwitch();
      _currentTab = Math.Clamp(index, 0, _tabs.Count()-1);
      OnAfterSwitch();

      GetContent().Data.ShapeContext = GetContent().Content;

      Settings.LastFile = index;
    }
    public EditorContent GetContent() => _tabs[_currentTab];

    public void AddTab(IPropertied content, bool isSwitch=true)
    {
      Console.WriteLine("Adding content on tabs");
      var meta = new EditorTabMetadata();
      meta.Filename = content.Name; 

      if (content is Sheet) 
        meta.Type = EditorContentType.Sheet;

      else if (content is World) 
        meta.Type = EditorContentType.World;

      else throw new Exception();

      // This file already exist within the tab files
      if (Settings.LastFiles.Find((file)=>file.Filename == meta.Filename) != null)
      {
        Console.WriteLine("Cannot add Content. Already exist.");
        return;
      }
      Settings.LastFiles.Add(meta);

      _tabs.Add(new EditorContent(content));
      _tabs.Last().Data.ShapeContext = content;

      // First tab in the list yet
      if (_tabs.Count() == 1 || isSwitch) Switch(_tabs.Count()-1);
    }
  }


	public class Editor : Nez.Entity
	{
    public EditorSettings Settings = new EditorSettings();

    public Selection Selection;
    public Serializer Serializer;
    
    Settings _settingsWindow;

    public ImGuiWinManager WindowManager;
    public ContentManager ContentManager;

    void OnCloseContent()
    {
      // Store last state
      ContentManager.GetContent().Data.Zoom = Scene.Camera.RawZoom;
      ContentManager.GetContent().Data.Position = Scene.Camera.Position;
    }
    void OnOpenContent()
    {
      // Restore last state
      Scene.Camera.RawZoom = ContentManager.GetContent().Data.Zoom;
      Scene.Camera.Position = ContentManager.GetContent().Data.Position;
    }
    public override void OnAddedToScene()
    {
      Scene.Camera.Position = -Screen.Center / 2;

      ContentManager.OnBeforeSwitch += OnCloseContent;
      ContentManager.OnAfterSwitch += OnOpenContent;

      Selection = AddComponent(new Selection());
      Serializer = new Serializer(ContentManager);

      WindowManager = new ImGuiWinManager();
      WindowManager.Windows.Add(new Settings(Settings));

      Serializer.LoadStartup();

      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent()); 
      AddComponent(new SelectionRenderer(Selection, Settings.Colors));


      AddEditorComponent(
          new SheetSelector(),
          new Overlays(),
          new Menubar(),
          new StatusBar(),
          new SpriteSceneView(),
          new ShapeAnnotator(),
          new AnimationEditor(),
          new WorldEditor());

      Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().RegisterDrawCommand(RenderImGui);
    }
    void RenderImGui()
    {
      WindowManager.Render(this);
      ContentManager.Render(this);
    }
    public override void Update()
    {
      base.Update();
      Scene.ClearColor = Settings.Colors.Background.ToColor();

      ContentManager.Update();
    }
    public override void OnRemovedFromScene()
    {
      Serializer.SaveSettings();
    }
        
	}
}

