using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet
{
	public class Editor : Nez.Entity
	{
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
      public virtual void OnEditorUpdate() {}
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
    
    public override void OnAddedToScene()
    {
      Name = "SpriteSheet Editor";
      Set(EditingState.Default);
      SpriteSheet = new Sheet(_gui.LoadTexture("Assets/Raw/Unprocessed/export/test_canvas.png"));
      _gui.ShapeContext = SpriteSheet;
      Position = Screen.Center;
      AddSubEntity(new SheetView());
    }
    public void AddSubEntity(SubEntity entity) 
    {
      entity.Init(this);
      entity.SetParent(this);
      _children.AddIfNotPresent(entity);        
      Scene.AddEntity(entity);
    }
    public T GetSubEntity<T>() => (T)_children.OfType<T>().First();
    public void Set(EditingState state) { EditState = state; ImGui.SetNextWindowFocus(); }
    public override void Update()
    {
      HandleGuiDrags();
      foreach (var child in _children) child.OnEditorUpdate();
    }
    void HandleGuiDrags() 
    {
      var pos = ImGui.GetIO().MousePos;
      if (_gui.IsDrag) 
      {
        _gui.IsDragFirst = false;
        _gui.MouseDragArea.Size = new Vector2(
            pos.X - _gui.MouseDragArea.X, 
            pos.Y - _gui.MouseDragArea.Y);
        _gui.MouseDragArea = _gui.MouseDragArea.ConsumePoint(pos);
      }
      for (int i = 0; i < 3; i++)
      {
        if (ImGui.GetIO().MouseDown[i] && !_gui.IsDrag)
        {
          _gui.IsDrag = true;
          _gui.IsDragFirst = true;
          _gui.MouseDragButton = i;
          _gui.MouseDragArea.X = pos.X;
          _gui.MouseDragArea.Y = pos.Y;
          _gui.MouseDragStart.X = pos.X;
          _gui.MouseDragStart.Y = pos.Y;
          break;
        }
      }
      _gui.IsDragLast = false;
      if (_gui.IsDrag && ImGui.GetIO().MouseReleased[_gui.MouseDragButton]) 
      {
        _gui.IsDrag = false;
        _gui.IsDragLast = true;
      }
    }
    ~Editor()
		{ 
      ImUtils.UnbindLastTexture();
		}

	}
}

