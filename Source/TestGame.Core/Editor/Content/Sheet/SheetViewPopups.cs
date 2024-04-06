using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;

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
        if (ImGui.MenuItem(IconFonts.FontAwesome5.PlusSquare + " Convert to SpriteScene"))
        {
          imgui.NameModal.Open((name)=>
          {
              var spriteScene = _sheet.CreateSpriteScene(name, ContentData.SelectionList.Last() as Sprite);
              _sheet.SpriteScenees.AddIfNotPresent(spriteScene);
              if (OnConvertToScene != null) OnConvertToScene(spriteScene);
          });
        }
        // add to exisiting spriteScene; select by list
        if (_sheet.SpriteScenees.Count() > 0 && ImGui.BeginMenu(IconFonts.FontAwesome5.UserPlus + " Add to SpriteScene"))
        {
          foreach (var spriteScene in _sheet.SpriteScenees)
          {
            if (ImGui.MenuItem(IconFonts.FontAwesome5.Users + " " + spriteScene.Name)) 
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