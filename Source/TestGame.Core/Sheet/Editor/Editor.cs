using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace Raven.Sheet
{
	public class Editor : Nez.Entity, IPropertied
	{
    public PropertyList Properties { get; set; } = new PropertyList();
    string IPropertied.Name { get; set; } = "SpriteSheet Editor";
    public enum EditingState 
    { 
      AutoRegion, 
      SelectedSprite, 
      Inactive, 
      AnnotateShape, 
      Default, 
      Modal 
    };
    public class SubEntity : Nez.Entity 
    {
      public GuiData Gui { get => Editor.GetCurrentState(); }
      public Editor Editor { get; private set; }
      
      public void Init(Editor editor) => Editor = editor; 
      public virtual void OnChangedTab() {}
      public virtual void OnDisableTab() {}
      public abstract class RenderableComponent<T> : Nez.RenderableComponent where T : SubEntity 
      { 
        public T Parent { get => Entity as T; }
        public GuiData Gui { get => Parent.Gui; }
        public Editor Editor { get => Parent.Editor; }
        public override RectangleF Bounds 
        {
          get
          {
            if (_areBoundsDirty && _bounds.Width != 0)
            {
              _bounds.CalculateBounds(Entity.Position, _localOffset, new Vector2(_bounds.Width, _bounds.Height)/2, 
                  Entity.Scale, Entity.Rotation, _bounds.Width, _bounds.Height); 
              _areBoundsDirty = false;
            }
            else _bounds = new RectangleF(-5000, -5000, 10000, 10000);
            return _bounds;
          }
        }       
        public void SetBounds(RectangleF bounds) => _bounds = bounds;
      }
    }
    public class SheetEntity : SubEntity
    { 
      public abstract class Renderable<T> : SubEntity.RenderableComponent<T> where T : SheetEntity
      {  
        public SheetGui SheetGui { get => Parent.SheetGui; }
        public Sheet Sheet { get => Parent.Sheet; }
      }
      public Sheet Sheet { get => SheetGui._sheet; }
      public SheetGui SheetGui { get => Editor.GetCurrentGui() as SheetGui; }
    }
    public class WorldEntity : SubEntity
    { 
      public abstract class Renderable<T> : SubEntity.RenderableComponent<T> where T : WorldEntity
      {  
        public World World { get => Parent.World; }
        public WorldGui WorldGui { get => Parent.WorldGui; }

      }
      public World World { get => WorldGui._world; }
      public WorldGui WorldGui { get => Editor.GetCurrentGui() as WorldGui; }
    }
    private List<SubEntity> _children = new List<SubEntity>();

    public EditingState EditState = EditingState.Default; 
    public EditingState PrevEditState = EditingState.Default; 
    public GuiColors ColorSet = new GuiColors();

    internal List<IPropertied> _tabs = new List<IPropertied>();
    public List<GuiData> _tabsState = new List<GuiData>();
    private int _currentTab = 0;
    public static int ScreenRenderLayer = -2;
    public static int WorldRenderLayer = 0;

    public override void OnAddedToScene()
    {
      AddTab(new Sheet("Assets/Raw/Unprocessed/export/test_canvas.png"));
      var world = new World();
      world.Name = "sample.world";
      world.AddSheet(GetCurrent() as Sheet);
      AddTab(world);
      Scene.AddRenderer(new ScreenSpaceRenderer(-2, ScreenRenderLayer));
  
      AddSubEntity(
          new Selection(),
          new SheetView(),
          new SheetSelector(),
          new PropertiesRenderer(),
          new ViewMenubar(),
          new ViewStatbar(),
          new SpritexView(),
          new SpritexLister(),
          new Annotator(),

          new WorldView());
      Switch(0); 
    }
    public void AddSubEntity(params SubEntity[] entities) 
    {
      foreach (var entity in entities)
      {
        entity.Init(this);
        entity.SetParent(this);
        _children.AddIfNotPresent(entity);        
        Scene.AddEntity(entity);
      }
    }
    public void RenderImGui(PropertiesRenderer renderer)
    {
      _tabs[_currentTab].RenderImGui(renderer);
      DrawFilePicker();
      DrawNameModal();
    }
    public void Set(EditingState state) 
    { 
      EditState = state;  
      switch (state)
      {
        case EditingState.AnnotateShape: Mouse.SetCursor(MouseCursor.Crosshair); break;
        case EditingState.SelectedSprite: Mouse.SetCursor(MouseCursor.SizeAll); break;
        default: Mouse.SetCursor(MouseCursor.Arrow); break;
      }
    }
    public object GetCurrent() 
    {
      if (_tabs[_currentTab] is WorldGui world) return world._world; 
      else if (_tabs[_currentTab] is SheetGui sheet) return sheet._sheet; 
      throw new Exception();
    }
    public IPropertied GetCurrentGui() => _tabs[_currentTab];
    public GuiData GetCurrentState() => _tabsState[_currentTab];
    public T GetSubEntity<T>() where T: SubEntity => (T)_children.OfType<T>().First();    
    public void Switch(int index) 
    {
      // Store last state
      GetCurrentState().Zoom = Scene.Camera.RawZoom;
      GetCurrentState().Position = Scene.Camera.Position;

      // Clean
      GetSubEntity<Selection>().End();
      GetSubEntity<SheetSelector>().RemoveSelection();

      _currentTab = Math.Clamp(index, 0, _tabs.Count()-1);

      // Restore last state
      Scene.Camera.RawZoom = GetCurrentState().Zoom;
      Scene.Camera.Position = GetCurrentState().Position;


      foreach (var child in _children) 
      {
        if ((child is SheetEntity && GetCurrentGui() is SheetGui) 
         || (child is WorldEntity && GetCurrentGui() is WorldGui)
         || (child is SubEntity && !(child is SheetEntity) && !(child is WorldEntity)))
        {
          Console.WriteLine($"Enabled {child.GetType().Name}");
          if (GetCurrent() is Component component) component.Entity = this;
          else if (GetCurrent() is Entity entity) entity.Scene = Scene;
          child.Enabled = true;
          child.OnChangedTab();
        }
        else 
        {
          Console.WriteLine($"Disabled {child.GetType().Name}");
          child.Enabled = false;
          child.OnDisableTab();
        }
      }
      GetCurrentState().ShapeContext = GetCurrentGui();

    }
    public void Save() {}
    public void AddTab(Sheet sheet) => AddTab(new SheetGui(sheet), new GuiData());
    public void AddTab(World world) => AddTab(new WorldGui(world), new WorldGuiData());
    void AddTab(Propertied content, GuiData gui)
    {
      _tabs.Add(content);
      _tabsState.Add(gui);
      _tabsState.Last().ShapeContext = content;
    }
    Action<string> _pickerCallback = null;
    Action<string> _nameCallback = null;
    bool _isOpenFile = false;
    bool _isNameModal = false;
    string _nameModalInput = "";
    public void OpenFilePicker(Action<string> callback) 
    {
      _pickerCallback = callback;
      _isOpenFile = true;
    }
    public void OpenNameModal(Action<string> callback) 
    {
      _nameCallback = callback;
      _isNameModal = true;
    }
    public void OpenProjectSettings() {}
    void DrawNameModal()
    {
      if (_isNameModal)
      {
        ImGui.OpenPopup("name-action-modal");
        _isNameModal = false;
      }
      var open = true;
      if (ImGui.BeginPopupModal("name-action-modal", ref open, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize))
      {
        Set(Editor.EditingState.Modal);
        ImGui.SetKeyboardFocusHere();
        if (ImGui.InputText("Name", ref _nameModalInput, 50, ImGuiInputTextFlags.EnterReturnsTrue))
        {
          _nameCallback.Invoke(_nameModalInput);
          Set(Editor.EditingState.Default);
          ImGui.CloseCurrentPopup();
          _nameModalInput = "";
        }
        ImGui.EndPopup();
      }

    }
    void DrawFilePicker()
    {
      if (_isOpenFile)
      {
        ImGui.OpenPopup("file-picker-modal");
        _isOpenFile = false;
      }
      var isOpen = true;
      if (ImGui.BeginPopupModal("file-picker-modal", ref isOpen, ImGuiWindowFlags.NoTitleBar))
      {
        Set(EditingState.Modal);
        var picker = Nez.ImGuiTools.FilePicker.GetFilePicker(this, Path.Combine(Environment.CurrentDirectory, "Content"), ".png|.atlas");
        picker.DontAllowTraverselBeyondRootFolder = true;
        if (picker.Draw())
        {
          _pickerCallback.Invoke(picker.SelectedFile);
          _pickerCallback = null;
          Set(EditingState.Default);
          Nez.ImGuiTools.FilePicker.RemoveFilePicker(this);
        }
        ImGui.EndPopup();
      }
    }
	}
}

