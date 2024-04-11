using ImGuiNET;
using Nez;
using Icon = IconFonts.FontAwesome5;

namespace Raven
{
	  public class SheetViewPopup : EditorInterface, IImGuiRenderable
  {
    Sheet _sheet { get => Content as Sheet; }
    SpriteScene _spriteSceneOnName;
    bool _isOpenSpriteOptions = false;

    public event Action<SpriteScene> OnConvertToScene;

    public void OpenSpriteOptions() => _isOpenSpriteOptions = true;

    void IImGuiRenderable.Render(Raven.ImGuiWinManager imgui)
    {
      if (_isOpenSpriteOptions)
      {
        _isOpenSpriteOptions = false;
        ImGui.OpenPopup("sprite-popup");
      }

      // something is selected; render options
      if (ContentData.SelectionList.NotEmpty() && ContentData.SelectionList.Last() is Sprite sprite && ImGui.BeginPopup("sprite-popup"))
      {
        // convert to new spriteScene
        if (ImGui.MenuItem(Icon.SyncAlt + "   Convert To SpriteScene"))
        {
          void Convert()
          {
            imgui.NameModal.Open((name)=>
            {
              var spriteScene = _sheet.CreateSpriteScene(name, ContentData.SelectionList.Last() as Sprite);

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
