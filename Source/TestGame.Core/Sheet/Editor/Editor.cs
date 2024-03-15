using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet
{
	public class Editor : Nez.Entity, IPropertied
	{
    public PropertyList Properties { get; set; } = new PropertyList();
    string IPropertied.Name { get; set; } = "";
    public enum EditingState { AutoRegion, SelectedSprite, Inactive, AnnotateShape, Default, Modal };
    public class SubEntity : Nez.Entity 
    {
      public GuiData Gui { get; private set; }
      public Editor Editor { get; private set; }
      public void Init(Editor editor)
      {
        Editor = editor;
        Gui = editor._gui; // Such a thing
      } 
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
    private List<SubEntity> _children = new List<SubEntity>();

    private GuiData _gui = new GuiData();
    [Inspectable]
    public GuiColors ColorSet = new GuiColors();
    public int TileWidth => SpriteSheet.TileHeight;
    public int TileHeight => SpriteSheet.TileHeight;

    public EditingState EditState = EditingState.Default; 
    public EditingState PrevEditState = EditingState.Default; 
    public Sheet SpriteSheet = null; 
    public static int ScreenRenderLayer = -2;
    public static int WorldRenderLayer = 0;

    public override void OnAddedToScene()
    {
      Name = "SpriteSheet Editor";
      Set(EditingState.Default);
      SpriteSheet = new Sheet(_gui.LoadTexture("Assets/Raw/Unprocessed/export/test_canvas.png"));
      SpriteSheet.SetTileSize(16, 16);
      _gui.ShapeContext = SpriteSheet;
      Scene.AddRenderer(new ScreenSpaceRenderer(-2, ScreenRenderLayer));
        
      AddSubEntity(new Selection());
      AddSubEntity(new SheetView());
      AddSubEntity(new SheetSelector());
      AddSubEntity(new PropertiesRenderer());
      AddSubEntity(new ViewMenubar());
      AddSubEntity(new SpritexView());
      AddSubEntity(new SpritexLister());
      AddSubEntity(new Annotator());

    }
    public void AddSubEntity(SubEntity entity) 
    {
      entity.Init(this);
      entity.SetParent(this);
      _children.AddIfNotPresent(entity);        
      Scene.AddEntity(entity);
    }
    public void RenderImGui(PropertiesRenderer renderer)
    {
      ImGui.Begin(IconFonts.FontAwesome5.ThLarge + " " + GetType().Name, ImGuiWindowFlags.NoCollapse);
      SpriteSheet.RenderImGui(renderer);
      IPropertied.HandleNewProperty(SpriteSheet, this);
      IPropertied.RenderProperties(SpriteSheet);
      ImGui.End();
    }
    
    public T GetSubEntity<T>() => (T)_children.OfType<T>().First();
    public void Set(EditingState state) { EditState = state;  }

	}
}

