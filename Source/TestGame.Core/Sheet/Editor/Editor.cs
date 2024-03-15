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
        public Sheet Sheet { get => Parent.Sheet; }
      }
      public Sheet Sheet { get => Editor.GetCurrent() as Sheet; }
    }
    private List<SubEntity> _children = new List<SubEntity>();

    public EditingState EditState = EditingState.Default; 
    public EditingState PrevEditState = EditingState.Default; 
    public GuiColors ColorSet = new GuiColors();

    public List<IPropertied> _tabs = new List<IPropertied>();
    public List<GuiData> _tabsState = new List<GuiData>();
    private int _currentTab = 0;
    public static int ScreenRenderLayer = -2;
    public static int WorldRenderLayer = 0;

    public override void OnAddedToScene()
    {
      AddTab(new Sheet("Assets/Raw/Unprocessed/export/test_canvas.png"));
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
          new Annotator());

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
    public IPropertied GetCurrent() => _tabs[_currentTab];
    public GuiData GetCurrentState() => _tabsState[_currentTab];
    public T GetSubEntity<T>() where T: SubEntity => (T)_children.OfType<T>().First();    
    public T GetTab<T>(int index) where T: Sheet => (T)_tabs[index];
    public void Switch(int index) 
    {
      _currentTab = Math.Clamp(index, 0, _tabs.Count());
      foreach (var child in _children) 
      {
        if ((child is SheetEntity && GetCurrent() is Sheet) || child is SubEntity)
        {
          child.Enabled = true;
          child.OnChangedTab();
        }
        else child.Enabled = false;
      }
    }
    public void AddTab<T>(T content) where T: Sheet 
    {
      _tabs.Add(content);
      _tabsState.Add(new GuiData());
      _tabsState.Last().ShapeContext = content;
    } 

	}
}

