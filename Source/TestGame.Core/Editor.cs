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

    public bool HasContent { get => _tabs.Count() > 0; }

    public override void OnAddedToScene()
    {
      PrimitiveBatch = new PrimitiveBatch();
      Scene.Camera.Position = -Screen.Center / 2;

      AddEditorComponent(new Serializer(), new Settings(), new Selection());
      Component<Serializer>().LoadStartup();

      Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().RegisterDrawCommand(RenderImGui);
      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent()); 
      AddEditorComponent(
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

      foreach (var child in _children) 
      {
        child.OnContent();
      }
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
      Scene.ClearColor = Settings.Colors.Background.ToColor();
      if (HasContent)
      {
        Settings.LastFiles[_currentTab].Filename = GetContent().Content.Name;
        GetContent().Data.Filename = GetContent().Content.Name;
      }
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
    public T Component<T>() where T: EditorComponent => (T)_children.OfType<T>().First();    


    public void Switch(int index) 
    {
      Console.WriteLine($"Switched to {index}");
      // Store last state
      GetContent().Data.Zoom = Scene.Camera.RawZoom;
      GetContent().Data.Position = Scene.Camera.Position;

      try 
      {
        GetComponent<Utils.Components.CameraMoveComponent>().Enabled = false;
        GetComponent<Utils.Components.CameraZoomComponent>().Enabled = false;

        // Clean
        GetEditorComponent<Selection>()?.End();
        GetEditorComponent<SheetSelector>()?.RemoveSelection();
      }
      catch (Exception) {}

      _currentTab = Math.Clamp(index, 0, _tabs.Count()-1);

      // Restore last state
      Scene.Camera.RawZoom = GetContent().Data.Zoom;
      Scene.Camera.Position = GetContent().Data.Position;


      foreach (var child in _children) 
      {
        child.OnContent();
      }
      GetContent().Data.ShapeContext = GetContent().Content;

      Settings.LastFile = index;

    }
    public void AddTab(IPropertied content, bool isSwitch=true)
    {
      Console.WriteLine("Adding content on tabs");
      var meta = new EditorTabMetadata();
      meta.Filename = content.Name; 
      if (content is Sheet.Sheet) meta.Type = EditorContentType.Sheet;
      else if (content is World) meta.Type = EditorContentType.World;
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


      if (content is Entity entity && !Scene.Entities.Contains(entity)) Scene.AddEntity(entity);

      // First tab in the list yet
      if (_tabs.Count() == 1 || isSwitch) Switch(_tabs.Count()-1);
    }
    public override void OnRemovedFromScene()
    {
      Component<Serializer>().SaveSettings();
    }
        
	}
}

