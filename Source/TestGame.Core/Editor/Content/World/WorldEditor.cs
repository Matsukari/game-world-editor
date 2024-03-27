using Nez;


namespace Raven.Sheet
{
  public enum PaintType { Single, Rectangle, Line, Fill }
  public enum PaintMode { Pen, Eraser }
  // <summary>
  // This manages the windows to modify and edit the world, its levels and layers, as opposed to WorldView that handles 
  // selection or direct manipulation to the world
  // </summary>
  public class WorldEditor : EditorComponent, IImGuiRenderable
  {
    // Essensials
    internal World _world;
    internal WorldInspector _inspector = new WorldInspector();
    int _selectedLevel = -1;

    // Fomr sprite picker
    public object SelectedSprite { get => _spritePicker.SelectedSprite; }
    internal SpritePicker _spritePicker = new SpritePicker();

    // Settings
    public bool IsDrawTileLayerGrid = true;
    public bool IsRandomPaint = false;
    public PaintMode PaintMode = PaintMode.Pen;
    public PaintType PaintType = PaintType.Single;

    public bool CanPaint { get => SelectedSprite != null; }

    // Wrapped objects
    internal List<LevelInspector> _levelInspectors = new List<LevelInspector>();
    TilePainter _tilePainter;

    public int SelectedLevel 
    { 
      get => _selectedLevel; 
      set 
      {
        SyncLevelInspectors();
        if (value < -1 || value >= _world.Levels.Count) throw new IndexOutOfRangeException();
        _selectedLevel = value; 
        foreach (var inspector in _levelInspectors) inspector.Selected = false;
        if (value >= 0)
        {
          _levelInspectors[SelectedLevel].Selected = true;
          _world.CurrentLevel = _world.Levels[value]; 
        }
      }
    }
    public override void OnContent()
    {
      if (RestrictTo<World>())
      {
        _world = Content as World;
        _tilePainter = new TilePainter(this, Editor);
        _spritePicker.HandleSelectedSprite = _tilePainter.HandleSelectedSprite;
      }
    }
        
    void SyncLevelInspectors()
    {
      if (_levelInspectors.Count() != _world.Levels.Count())
      {
        _levelInspectors.Clear();
        foreach (var level in _world.Levels) 
        {
          _levelInspectors.Add(new LevelInspector(level, this));
          if (_world.CurrentLevel != null && _world.CurrentLevel.Name == level.Name) _levelInspectors.Last().Selected = true;
        }
      }
    }
    public void Render(Editor editor)
    {
      SyncLevelInspectors();
      _inspector.WorldEditor = this;
      _inspector.Render(editor);

      // Render window of current level
      if (SelectedLevel != -1) _levelInspectors[SelectedLevel].Render(editor);

      _spritePicker.Draw(new Nez.RectangleF(0f, Screen.Height-450-28, 450, 450));


    }
  }
}

