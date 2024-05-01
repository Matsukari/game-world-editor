using ImGuiNET;
using Icon = IconFonts.FontAwesome5;

namespace Raven
{
	  public class SheetViewPopup : EditorInterface, IImGuiRenderable
  {
    Sheet _sheet { get => Content as Sheet; }
    SpriteScene _spriteSceneOnName;
    bool _isOpenSpriteOptions = false;
    bool _isOpenTileOptions = false;
    public Widget.SpriteSlicer SpriteSlicer = new Widget.SpriteSlicer();
    public Widget.AnnotatorPane AnnotatorPane;

    public event Action<SpriteScene> OnConvertToScene;
    public event Action<AnimatedSprite> OnCreateAnimation;

    public void OpenSpriteOptions() => _isOpenSpriteOptions = true;
    public void OpenTileOptions() => _isOpenTileOptions = true;

    public override void Initialize(Editor editor, EditorContent content)
    {
      base.Initialize(editor, content);
      AnnotatorPane = new Widget.AnnotatorPane(Settings.Colors);
    }
    void IImGuiRenderable.Render(Raven.ImGuiWinManager imgui)
    {
      if (_isOpenSpriteOptions)
      {
        _isOpenSpriteOptions = false;
        ImGui.OpenPopup("sprite-popup");
      }
      if (_isOpenTileOptions)
      {
        _isOpenTileOptions = false;
        ImGui.OpenPopup("Tile-popup");
      }
      SpriteSlicer.Draw();
      AnnotatorPane.Render(imgui);

      void ConvertOption(Sprite sprite)
      {
        if (ImGui.MenuItem(Icon.SyncAlt + "   Convert To SpriteScene"))
        {
          void Convert()
          {
            imgui.NameModal.Open((name)=>
            {
              var spriteScene = _sheet.CreateSpriteScene(name, sprite);

                // name conflict 
              if (_sheet.SpriteScenees.Find(item => item.Name == name) != null)
              {
                imgui.ConfirmModal.Open((state)=>
                {
                  if (state == ConfirmState.Refused) Convert();
                  else if (state == ConfirmState.Confirmed) 
                  {
                    _sheet.ReplaceScene(name, spriteScene);
                    if (OnConvertToScene != null) OnConvertToScene(spriteScene);
                  }
                }, "Warning", $"File '{name}' already exists. \noverwrite it?");
              }
              else 
              {
                _sheet.AddScene(spriteScene);
                if (OnConvertToScene != null) OnConvertToScene(spriteScene);
              }
            });
          }
          Convert();
        }
      }

      // something is selected; render options
      if (ContentData.SelectionList.NotEmpty() && ContentData.SelectionList.Last() is Tile tile && ImGui.BeginPopup("Tile-popup"))
      {
        ConvertOption(new Sprite(tile.Region, tile._sheet));
        if (ImGui.MenuItem(Icon.Shapes + "   Embed Shape"))
        {
          AnnotatorPane.Edit(new SourcedSprite(sprite: new Sprite(tile.Region, _sheet)), tile, shape => _sheet.CreateTile(tile));
        }
        ImGui.EndPopup();
      }
      if (ContentData.SelectionList.NotEmpty() && ContentData.SelectionList.Last() is Sprite sprite && ImGui.BeginPopup("sprite-popup"))
      {
        ConvertOption(sprite);
        // convert to new spriteScene
        if (ImGui.MenuItem(Icon.Film + "   Create Animation"))
        {
          SpriteSlicer.Sprite = sprite;
          SpriteSlicer.Open(()=>_sheet.AddAnimation(new AnimatedSprite(sprite.SubDivide(SpriteSlicer.SplitSize))));
           
        }
        // add to exisiting spriteScene; select by list
        if (_sheet.SpriteScenees.Count() > 0 && ImGui.BeginMenu(Icon.Plus + "   Add To SpriteScene"))
        {
          foreach (var spriteScene in _sheet.SpriteScenees)
          {
            if (ImGui.MenuItem(Icon.Users + "   " + spriteScene.Name)) 
            {
              _spriteSceneOnName = spriteScene;
              imgui.NameModal.Open((name)=>
              {
                _spriteSceneOnName.AddSprite(name, new SourcedSprite(sprite as Sprite));
              });
            }
          }
          ImGui.EndMenu();
        }
        ImGui.EndPopup();
      } 
    }
  }

}
