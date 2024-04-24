using Nez;


namespace Raven
{
	
  public class SheetViewImGui : EditorInterface, IImGuiRenderable
  {
    readonly EditorSettings _settings;
    public readonly SheetInspector Inspector;
    public readonly SheetViewPopup Popups;
    public SpriteSceneView SceneView;
    public SpriteAnimationEditor SpriteAnimEditor;
    public SheetObjectInspector ObjectInspector = new SheetObjectInspector();
    TileInspector _tileInspector = new TileInspector();
    SpriteInspector _spriteInspector = new SpriteInspector();

    SelectionList _list;
    Sheet _sheet;

    public SheetViewImGui(EditorSettings settings, Camera camera)
    {
      _settings = settings;
      Inspector = new SheetInspector(settings, camera);
      Inspector.SpritePicker.OnDropSource += source => 
      {
        source.SpriteScene = SceneView.LastSprite.SpriteScene;
        SceneView.LastSprite.SpriteScene.AddSprite(source);
      };
      Popups = new SheetViewPopup();

      Inspector.OnClickAnimation += anim => SpriteAnimEditor.Open(anim);
      Inspector.OnDeleteAnimation += anim => SpriteAnimEditor.Close();
    }
    public override void Initialize(Editor editor, EditorContent content)
    {
      base.Initialize(editor, content);
      SpriteAnimEditor = new SpriteAnimationEditor(content.Content as Sheet);
    }
        
    public void Update(Sheet sheet, SelectionList list) 
    {
      _list = list;
      _sheet = sheet;
    }

    void IImGuiRenderable.Render(ImGuiWinManager imgui)
    {
      Inspector.Sheet = _sheet;
      Inspector.Render(imgui);

      var animEditor = SpriteAnimEditor as IImGuiRenderable;
      animEditor.Render(imgui);

      if (SceneView != null && SceneView.IsEditing)
      {
        SceneView.SceneInspector.Render(imgui);
        SceneView.AnnotatorPane.Render(imgui);
        (SceneView.AnimationEditor as IImGuiRenderable).Render(imgui);
        return;
      }
      else if (_list != null && _list.Selections.Count() > 0)
      {
        // Evaluate to either one; 
        _tileInspector.Tile = _list.Selections.Last() as Tile;
        _spriteInspector.Sprite = _list.Selections.Last() as Sprite;

        if (_tileInspector.Tile != null) ObjectInspector.Inspector = _tileInspector;
        else if (_spriteInspector.Sprite != null) ObjectInspector.Inspector = _spriteInspector;

        ObjectInspector.Render(imgui);
      }


      var popup = Popups as IImGuiRenderable;
      popup.Render(imgui);
    }

  }

}
