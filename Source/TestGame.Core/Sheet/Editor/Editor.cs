using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ImGuiTools;

namespace Raven.Sheet
{
  public abstract class ImGuiWindow : Entity 
  {
    public abstract void RenderImGui();
  }
  /// <summary>
  /// Operates on a single spritesheet 
  /// </summary>
	public class Editor
	{
    public enum EditingState { AutoRegion, SelectedSprite, Inactive, AnnotateShape, Default };
    public interface Component 
    {
      public void OnEditorUpdate();
    }
    public abstract class Renderable : RenderableComponent, Editor.Component
    {
      public void OnEditorUpdate() {}
    } 
    public abstract class Window : ImGuiWindow
    { 
      protected GuiData Gui;
      protected Editor Editor;
      public void Init(Editor editor, GuiData gui) 
      {
        Editor = editor;
        Gui = gui; 
      }
      public virtual void Render() {} 
    }
    private List<Component> _components = new List<Component>();

    private GuiData _gui = new GuiData();
    public GuiColors ColorSet = new GuiColors();
    public int TileWidth => SpriteSheet.TileWidth;
    public int TileHeight => SpriteSheet.TileHeight;

    public EditingState EditState = EditingState.Default; 
    public EditingState PrevEditState = EditingState.Default; 
    public SpriteSheet SpriteSheet = null; 
    
		public SpriteSheetEditor()
		{
      SpriteSheet = new SpriteSheetData(_gui.LoadTexture("Assets/Raw/Unprocessed/export/test_canvas.png"));
      _gui.ShapeContext = SpriteSheet;
      Set(EditingState.AutoRegion);
		}
    public T GetComponent<T>() => (T)_components.OfType<T>().First();
    public void Set(EditingState state) { EditState = state; ImGui.SetNextWindowFocus(); }
    public void AddComponent(Control control) { _components.Add(control); control.Init(this, _gui); }
    public override void OnAddedToEntity() 
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(Render);    
      Entity.Scene.AddEntity(_selComplex);
    }
		public void Render()
		{
      HandleGuiDrags();
      foreach (var component in _components) component.Render();
      _selComplex.SheetUpdate();
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
    ~SpriteSheetEditor()
		{ 
      ImUtils.UnbindLastTexture();
      _selComplex.Destroy();
		}

	}
}

