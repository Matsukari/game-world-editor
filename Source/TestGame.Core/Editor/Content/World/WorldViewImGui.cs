
namespace Raven
{
  public enum PaintType { Single, Rectangle, Line, Fill }
  public enum PaintMode { Pen, Eraser }

  public class WorldViewImGui : EditorInterface, IImGuiRenderable
  {
    public SpritePicker SpritePicker { get => _spritePicker; }
    public List<LevelInspector> LevelInspectors { get => _levelInspectors; }
    public WorldViewPopup Popups { get => _popups;  }
    public readonly SceneInstanceInspector SceneInstanceInspector = new SceneInstanceInspector();

    readonly List<LevelInspector> _levelInspectors = new List<LevelInspector>();
    readonly TilePainter _spritePicker;
    readonly WorldInspector _inspector;
    readonly WorldViewPopup _popups;

    World _world;
    int _selectedLevel = -1;

    public WorldViewImGui(TilePainter painter)
    {
      _popups = new WorldViewPopup();
      _inspector = new WorldInspector(this);
      _inspector.OnAddSheet += file => _world.AddSheet(Serializer.LoadContent<Sheet>(file));
      _inspector.OnRemoveLevel += level => Selection.End();
     
      _spritePicker = painter;
    }

    public override void Initialize(Editor editor, EditorContent content)
    {
      base.Initialize(editor, content);
      _world = Content as World;
    }

    public LevelInspector SelectedLevelInspector { get => _levelInspectors.GetAtOrNull(SelectedLevel); }

    public int SelectedLevel 
    { 
      get => _selectedLevel; 
      set 
      {
        SyncLevelInspectors();
        if (value < -1 || value >= _world.Levels.Count) throw new IndexOutOfRangeException();
        if (value == -1) {_selectedLevel = -1; return;}

        foreach (var inspector in _levelInspectors) inspector.Selected = false;
        _selectedLevel = value; 
        _levelInspectors[_selectedLevel].Selected = true;
      }
    }
    
    void SyncLevelInspectors()
    {
      if (_levelInspectors.Count() != _world.Levels.Count())
      {
        _levelInspectors.Clear();
        foreach (var level in _world.Levels) 
        {
          _levelInspectors.Add(new LevelInspector(level));
        }
        try
        {
          _levelInspectors[SelectedLevel].Selected = true;
        }
        catch (Exception) 
        {
          _selectedLevel = -1;
        }
      }
    }
    void SyncSpritePicker()
    {
      if (_spritePicker.Sheets.Count() != _world.Sheets.Count())
      {
        _spritePicker.Sheets.Clear();
        foreach (var sheet in _world.Sheets) 
        {
          var data = new SheetPickerData(sheet);
          _spritePicker.Sheets.Add(data);
        }
      }
    }


    void IImGuiRenderable.Render(Raven.ImGuiWinManager imgui)
    {
      SyncSpritePicker();
      SyncLevelInspectors();

      _inspector.Render(imgui);
      SceneInstanceInspector.Render(imgui);

      // Render window of current level
      if (SelectedLevel != -1) _levelInspectors[SelectedLevel].Render(imgui);

      _popups.Update(SelectedLevelInspector?.CurrentLayer);
      (_popups as IImGuiRenderable).Render(imgui);


    }
  }
}

