using ImGuiNET;

namespace Raven
{
  public enum PaintType { Single, Rectangle, Line, Fill }
  public enum PaintMode { Pen, Eraser, Inspector, None }

  public class WorldViewImGui : EditorInterface, IImGuiRenderable
  {
    public TilePainter SpritePicker { get => _spritePicker; }
    public List<LevelInspector> LevelInspectors { get => _levelInspectors; }
    public WorldViewPopup Popups { get => _popups;  }
    public readonly SceneInstanceInspector SceneInstanceInspector = new SceneInstanceInspector();
    public readonly TileInstanceInspector TileInstanceInspector = new TileInstanceInspector();
    internal WindowHolder _objHolder;

    readonly List<LevelInspector> _levelInspectors = new List<LevelInspector>();
    readonly TilePainter _spritePicker;
    readonly WorldInspector _inspector;
    readonly WorldViewPopup _popups;

    World _world;
    int _selectedLevel = -1;

    public Layer CurrentLayer 
    {
      get 
      {
        var level = SelectedLevelInspector; 
        if (level == null) return null;
        return level.CurrentLayer; 
      }
    }

    public WorldViewImGui(TilePainter painter)
    {
      _popups = new WorldViewPopup();
      _popups.OnDeleteLevel += level => Nez.Core.GetGlobalManager<CommandManagerHead>().Current.Record(new RemoveLevelCommand(_world, level));
      _popups.OnPasteLevel += level => Nez.Core.GetGlobalManager<CommandManagerHead>().Current.Record(new AddLevelCommand(_world, level));
      _popups.OnCutLevel += level => Nez.Core.GetGlobalManager<CommandManagerHead>().Current.Record(new RemoveLevelCommand(_world, level));
      _popups.OnCutLevel += level => SelectOtherLevel();
      _popups.OnDeleteLevel += level => SelectOtherLevel();

      _inspector = new WorldInspector(this);
      // _inspector.OnAddSheet += file => _world.AddSheet(Serializer.LoadContent<Sheet>(file));
      _inspector.OnRemoveLevel += level => Selection.End();
      _inspector.OnRemoveLevel += level => SelectedLevel--;

      TileInstanceInspector.OnViewParent += ResetObj;
      SceneInstanceInspector.OnViewParent += ResetObj;
     
      _spritePicker = painter;
    }

    void ResetObj()
    {
      if (_objHolder != null && SelectedLevelInspector != null)
        _objHolder.Content = SelectedLevelInspector;
    }

    public override void Initialize(Editor editor, EditorContent content)
    {
      base.Initialize(editor, content);
      _world = Content as World;
      _popups.OnAnyPopupRender += OnAnyPopupRender;
    }

    public void SelectOtherLevel()
    {
      if (_world.Levels.Count() > 1 && SelectedLevelInspector != null)
      {
        var i = 0;
        foreach (var level in _world.Levels)
        {
          if (level.Name != SelectedLevelInspector.Level.Name) SelectedLevel = i;
          i++;
        }
      }
      else SelectedLevel = -1;
    }
    void OnAnyPopupRender(ImGuiWinManager imgui)
    {
      if (_spritePicker.SelectedSprite is Sprite && ImGui.MenuItem("Remove Selected Tile")) _spritePicker.SelectedSprite = null;
      if (_spritePicker.SelectedSprite is SpriteScene && ImGui.MenuItem("Remove Selected Scene")) _spritePicker.SelectedSprite = null;
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

        if (_objHolder != null && _objHolder.Content == null)
          _objHolder.Content = SelectedLevelInspector;
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



      imgui.GetRenderable<WindowHolder>("main").Content = _inspector;
      _objHolder = imgui.GetRenderable<WindowHolder>("sub");

      // Render window of current level
      if ((SelectedLevel != -1 && _objHolder.Content == null) || Nez.Input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape)) 
      {
        _objHolder.Content = SelectedLevelInspector;
      }

      _popups.Update(SelectedLevelInspector?.CurrentLayer);
      (_popups as IImGuiRenderable).Render(imgui);

      RenderAnnotations(_world);
      if (SelectedLevelInspector != null) RenderAnnotations(SelectedLevelInspector.Level);

    }
  }
}

