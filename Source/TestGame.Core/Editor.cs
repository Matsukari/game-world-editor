using Raven.Sheet;
using Nez;
using Raven.Serializers;

namespace Raven
{
  public enum EditingState { SelectedSprite, AnnotateShape, Default, Modal };

  // Implement along with EditorComponent to render imgui content
  public interface IImGuiRenderable 
  {
    public void Render(Editor editor);
  }


	public class Editor : Nez.Entity
	{
    public EditorSettings Settings = new EditorSettings();
    public EditingState EditState = EditingState.Default; 

    // All editor componnets
    List<EditorComponent> _children = new List<EditorComponent>();

    // Components that wants to render imgui content
    List<IImGuiRenderable> _imguiRenderables = new List<IImGuiRenderable>();

    // Either world or sheet; these are the objects that can be switch in and out
    internal List<EditorContent> _tabs = new List<EditorContent>();
    private int _currentTab = 0;

    // Common popups handled by the editor 
    public Widget.FilePicker FilePicker = new Widget.FilePicker();
    public Widget.NameModal NameModal = new Widget.NameModal();

    public static PrimitiveBatch PrimitiveBatch;

    public override void OnAddedToScene()
    {
      PrimitiveBatch = new PrimitiveBatch();

      if (File.Exists(file)) 
      {
        var loaded = new SheetSerializer().Load(file);
        AddTab(loaded);
      }
      else 
        AddTab(new Sheet.Sheet("Assets/Raw/Unprocessed/export/test_canvas.png"));

      Scene.Camera.Position = -Screen.Center / 2;
      var world = new World();
      world.Name = "sample.world";
      world.AddSheet(GetContent().Content as Sheet.Sheet);
      AddTab(world);
      Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().RegisterDrawCommand(RenderImGui);
      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent()); 
      AddComponent(new Settings(Settings));
      AddEditorComponent(
          new Selection(),
          new SheetView(),
          new SheetSelector(),
          new Overlays(),
          new Menubar(),
          new StatusBar(),
          new SpritexView(),
          new ShapeAnnotator(),
          new AnimationEditor(),
          new WorldView(),
          new WorldEditor());
      Switch(0); 
      Console.WriteLine("After swtch");
    }
    void RenderImGui()
    {
      foreach (var imgui in _imguiRenderables) 
      {
        if (imgui is Component component && !component.Enabled) continue;
        imgui.Render(this);
      }
      NameModal.Draw();
      FilePicker.Draw();
    }
    public override void Update()
    {
      base.Update();
      Scene.ClearColor = Settings.Colors.Background;
    }
    public void AddEditorComponent(params EditorComponent[] components) 
    {
      foreach (var component in components)
      {
        if (component is IImGuiRenderable) _imguiRenderables.Add(component as IImGuiRenderable);
        _children.AddIfNotPresent(component);        
        AddComponent(component);
      }
    }
    public EditorContent GetContent() => _tabs[_currentTab];

    public T GetEditorComponent<T>() where T: EditorComponent => (T)_children.OfType<T>().First();    

    public void Switch(int index) 
    {
      // Store last state
      GetContent().Data.Zoom = Scene.Camera.RawZoom;
      GetContent().Data.Position = Scene.Camera.Position;

      GetComponent<Utils.Components.CameraMoveComponent>().Enabled = false;
      GetComponent<Utils.Components.CameraZoomComponent>().Enabled = false;

      // Clean
      GetEditorComponent<Selection>()?.End();
      GetEditorComponent<SheetSelector>()?.RemoveSelection();

      _currentTab = Math.Clamp(index, 0, _tabs.Count()-1);

      // Restore last state
      Scene.Camera.RawZoom = GetContent().Data.Zoom;
      Scene.Camera.Position = GetContent().Data.Position;


      foreach (var child in _children) 
      {
        child.OnContent();
      }
      GetContent().Data.ShapeContext = GetContent().Content;

    }
    public void AddTab(IPropertied content)
    {
      _tabs.Add(new EditorContent(content));
      _tabs.Last().Data.ShapeContext = content;
      if (content is Entity entity && !Scene.Entities.Contains(entity)) Scene.AddEntity(entity);
    }
    string file = "Sample.sheet";
    public void Save() 
    {

      new SheetSerializer().Save(file, GetContent().Content as Sheet.Sheet);
    }
    public void OpenProjectSettings() {}
	}
}

