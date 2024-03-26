using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace Raven.Sheet
{
	public class Editor : Nez.Entity
	{
    public enum EditingState { SelectedSprite, AnnotateShape, Default, Modal };

    public class EditorComponent : RenderableComponent, IUpdatable
    {
      public GuiData Gui { get => Editor.GetCurrentState(); }
      public Editor Editor { get; private set; }
      
      public void Init(Editor editor) => Editor = editor; 
      public virtual void OnChangedTab() {}
      public virtual void OnDisableTab() {}
      public T Parent { get => Entity as T; }
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

    private List<EditorComponent> _children = new List<EditorComponent>();

    public EditingState EditState = EditingState.Default; 
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
  
      AddComponent(new Settings(ColorSet));
      AddEditorComponent(
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
    public override void Update()
    {
      base.Update();
      Scene.ClearColor = ColorSet.Background;
    }
    public void AddEditorComponent(params EditorComponent[] entities) 
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
    public T GetCurrentGui<T>() where T: Propertied => _tabs[_currentTab] as T;
    public GuiData GetCurrentState() => _tabsState[_currentTab];
    public T GetEditorComponent<T>() where T: EditorComponent => (T)_children.OfType<T>().First();    
    public void Switch(int index) 
    {
      // Store last state
      GetCurrentState().Zoom = Scene.Camera.RawZoom;
      GetCurrentState().Position = Scene.Camera.Position;

      // Clean
      GetEditorComponent<Selection>().End();
      GetEditorComponent<SheetSelector>().RemoveSelection();

      _currentTab = Math.Clamp(index, 0, _tabs.Count()-1);

      // Restore last state
      Scene.Camera.RawZoom = GetCurrentState().Zoom;
      Scene.Camera.Position = GetCurrentState().Position;


      foreach (var child in _children) 
      {
        if ((child is SheetEntity && GetCurrentGui() is SheetGui) 
         || (child is WorldEntity && GetCurrentGui() is WorldGui)
         || (child is EditorComponent && !(child is SheetEntity) && !(child is WorldEntity)))
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
    public void AddTab(World world) => AddTab(new WorldGui(this, world), new WorldGuiData());
    void AddTab(Propertied content, GuiData gui)
    {
      _tabs.Add(content);
      _tabsState.Add(gui);
      _tabsState.Last().ShapeContext = content;
    }
	}
}

