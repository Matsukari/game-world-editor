using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven
{
  public class SheetSelector : EditorComponent, IImGuiRenderable
  {
    Sheet _sheet;
    SpriteScene _spriteSceneOnName = null;
    public override void OnContent()
    {
      RemoveSelection();
      if (RestrictTo<Sheet>())
      {
        _sheet = Content as Sheet;
      }
    } 
    public void Render(Editor editor)
    {
      var spriteSceneView = Editor.GetEditorComponent<SpriteSceneView>();

      // right-click; open options
      if (Nez.Input.RightMouseButtonPressed && ContentData.Selection != null) 
      {
        ImGui.OpenPopup("sprite-popup");
      }

      void OpenPopup(string name)
      {
        ImGui.EndPopup();
        ImGui.CloseCurrentPopup();
        ImGui.OpenPopup(name);
      }
     
      // something is selected; render options
      if (ContentData.Selection is Sprites.Sprite sprite && ImGui.BeginPopup("sprite-popup"))
      {
        // convert to new spriteScene
        if (ImGui.MenuItem(IconFonts.FontAwesome5.PlusSquare + " Convert to SpriteScene"))
        {
          Editor.NameModal.Open((name)=>
          {
              var spriteScene = _sheet.CreateSpriteScene(name, ContentData.Selection as Sprites.Sprite);
              _sheet.SpriteScenees.AddIfNotPresent(spriteScene);
              Editor.GetEditorComponent<SpriteSceneView>().Edit(spriteScene);
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
              Editor.NameModal.Open((name)=>
              {
                _spriteSceneOnName.AddSprite(name, new Sprites.SourcedSprite(sprite: ContentData.Selection as Sprites.Sprite));
              });
            }
          }
          ImGui.EndMenu();
        }
        // // Has opened and made operation to last spriteScene' part
        // if (spriteSceneView.LastSprite != null)
        // {
        //   if (ImGui.MenuItem(IconFonts.FontAwesome5.UserPlus + " Add to last SpriteScene"))
        //   {
        //     _spriteSceneOnName = spriteSceneView.LastSprite.SpriteScene;
        //     OpenPopup("spriteScene-part-name");
        //     return;
        //   }
        //   if (spriteSceneView.LastSprite.ChangePart != null && ImGui.MenuItem(IconFonts.FontAwesome5.UserPlus + " Change last SpriteScene part"))
        //   {
        //     ImGui.EndPopup();
        //     ImGui.CloseCurrentPopup();
        //     spriteSceneView.LastSprite.SpriteScene.Parts.Data[spriteSceneView.LastSprite.ChangePart.Name] = new Sprites.SourcedSprite(spriteSceneView.LastSprite.SpriteScene, sprite);
        //     spriteSceneView.LastSprite.SpriteScene.Parts.Data[spriteSceneView.LastSprite.ChangePart.Name].Name = spriteSceneView.LastSprite.ChangePart.Name;
        //     spriteSceneView.Edit(spriteSceneView.LastSprite.SpriteScene);
        //     return;
        //   }
        // }
        ImGui.EndPopup();
      }


    } 
    public override void Update()
    {
      base.Update();
      HandleSheetInputs();
    }
    List<int> _selectedTiles = new List<int>();
    Vector2 _initialMouse = Vector2.Zero;
    public void RemoveSelection()
    {
      _selectedTiles.Clear(); 
      _initialMouse = Vector2.Zero;
      ContentData.Selection = null;
    }
    void HandleSheetInputs()
    { 
      var sheetView = Editor.GetEditorComponent<SheetView>();
      var input = Core.GetGlobalManager<Input.InputManager>();
      if (input.IsImGuiBlocking) return;

      // Foundation for multiple tile selection
      if (input.IsDragFirst && _initialMouse == Vector2.Zero && Nez.Input.LeftMouseButtonDown) 
      {
        _initialMouse = Entity.Scene.Camera.MouseToWorldPoint();
      }
      else if (input.IsDrag && _initialMouse != Vector2.Zero)
      {
        var mouseDragArea = new RectangleF();
        mouseDragArea.Location = sheetView.GetOffRegionInSheet(_initialMouse);
        mouseDragArea.Size = Entity.Scene.Camera.MouseToWorldPoint() - _initialMouse;
        mouseDragArea = mouseDragArea.AlwaysPositive();
        _selectedTiles = _sheet.GetTiles(mouseDragArea);
        if (_selectedTiles.Count > 1) 
        {
          var sprite = _sheet.CreateSprite(_selectedTiles.ToArray());
          Select(sprite);
        }
      }
      else if (input.IsDragLast)
      {
        var count = _selectedTiles.Count;
        _selectedTiles.Clear();
        _initialMouse = Vector2.Zero;
        if (count > 1) return;

        // Selects single tile
        if (!sheetView.TileInMouse.IsEmpty) 
        {
          var coord = _sheet.GetTile(sheetView.TileInMouse);
          var tileInCoord = _sheet.CustomTileExists(coord.X, coord.Y);
          if (tileInCoord != null) Select(tileInCoord);
          else Select(new Sprites.Tile(coord, _sheet));
        }
      }
    }
    public void Select(IPropertied sel)
    {
      if (ContentData.ShapeSelection != null || Editor.GetEditorComponent<Selection>().HasBegun()) return;
      ContentData.Selection = sel;
      Console.WriteLine($"Selected {ContentData.Selection.GetType().Name}");
    }

  }
}

