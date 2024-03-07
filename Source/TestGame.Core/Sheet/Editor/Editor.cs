using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet
{
	public class Editor : Nez.Entity, IPropertied
	{
    public PropertyList Properties { get; set; }
    string IPropertied.Name { get; set; }
    public enum EditingState { AutoRegion, SelectedSprite, Inactive, AnnotateShape, Default };
    public class SubEntity : Nez.Entity 
    {
      protected GuiData Gui { get; private set; }
      protected Editor Editor { get; private set; }
      public void Init(Editor editor)
      {
        Editor = editor;
        Gui = editor._gui; // Such a thing
      } 
      public abstract class RenderableComponent<T> : Nez.RenderableComponent where T : SubEntity 
      { 
        protected T Parent { get => Entity as T; }
        protected GuiData Gui { get => Parent.Gui; }
        protected Editor Editor { get => Parent.Editor; }
        public override RectangleF Bounds 
        {
          get
          {
            if (_areBoundsDirty)
            {
              _bounds.CalculateBounds(Entity.Position, _localOffset, new Vector2(_bounds.Width, _bounds.Height)/2, 
                  Entity.Scale, Entity.Rotation, _bounds.Width, _bounds.Height); 
              _areBoundsDirty = false;
            }
            return _bounds;
          }
        }       
        public void SetBounds(RectangleF bounds) => _bounds = bounds;
      }
    }
    private List<SubEntity> _children = new List<SubEntity>();

    private GuiData _gui = new GuiData();
    public GuiColors ColorSet = new GuiColors();
    public int TileWidth => SpriteSheet.TileHeight;
    public int TileHeight => SpriteSheet.TileHeight;

    public EditingState EditState = EditingState.Default; 
    public EditingState PrevEditState = EditingState.Default; 
    public Sheet SpriteSheet = null; 
    public static int ScreenRenderLayer = -1;
    public static int WorldRenderLayer = 0;
    
    public override void OnAddedToScene()
    {
      Name = "SpriteSheet Editor";
      Set(EditingState.Default);
      SpriteSheet = new Sheet(_gui.LoadTexture("Assets/Raw/Unprocessed/export/test_canvas.png"));
      SpriteSheet.SetTileSize(16, 16);
      _gui.ShapeContext = SpriteSheet;
      Scene.AddRenderer(new ScreenSpaceRenderer(-1, ScreenRenderLayer));
      Scene.AddRenderer(new RenderLayerRenderer(0, WorldRenderLayer));
        
      Position = Screen.Center;
      AddSubEntity(new SheetView());
      AddSubEntity(new SheetSelector());
      AddSubEntity(new PropertiesRenderer());
      AddSubEntity(new ViewMenubar());
      AddSubEntity(new Annotator());
    }
    public void AddSubEntity(SubEntity entity) 
    {
      entity.Init(this);
      entity.SetParent(this);
      _children.AddIfNotPresent(entity);        
      Scene.AddEntity(entity);
    }
    public void RenderImGui()
    {
      var name = "";
      int w = 0, h = 0;
      ImGui.Begin(GetType().Name);
      if (ImGui.InputText("Name", ref name, 10)) SpriteSheet.Name = name;
      if (ImGui.InputInt("TileWidth", ref w)) SpriteSheet.SetTileSize(w, TileHeight);
      if (ImGui.InputInt("TileHeight", ref h)) SpriteSheet.SetTileSize(TileWidth, h);
      ImGui.TextDisabled($"Editing: {EditState}");
      IPropertied.RenderProperties(this);
      ImGui.End();
    }
    public T GetSubEntity<T>() => (T)_children.OfType<T>().First();
    public void Set(EditingState state) { EditState = state;  }

	}
}

