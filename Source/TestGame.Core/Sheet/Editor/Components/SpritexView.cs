using Nez;
using ImGuiNET;
using Nez.Sprites;
using Nez.Textures;
using Microsoft.Xna.Framework;


namespace Raven.Sheet
{
  public class SpritexView : Editor.SubEntity
  {
    Entity _main;
    GuiData _gui;
    Editor _editor;
    Sprites.Spritex _spritex;
    float _zoom = 1;
    List<Entity> _entities = new List<Entity>();
    public SpritexView(GuiData gui, Editor editor)
    {
      _gui = gui;
      _editor = editor;
      Name = Names.SpritexView;
    }
    public void Edit(Sprites.Spritex spritex)
    {
      Position = Screen.Center;
      UnEdit();
      _spritex = spritex;
      _editor.GetComponent<SheetView>().IsCollapsed = true;
      _editor.Set(Editor.EditingState.SelectedSprite);
      var origin = AddComponent(new Guidelines.OriginLines());
      origin.Color = _editor.ColorSet.SpriteRegionActiveOutline;

      var mainEntity = Scene.CreateEntity(_spritex.Name);
      foreach (var part in _spritex.Body)
      {
        var partEntity = Scene.CreateEntity(_spritex.Name + part.SourceSprite.Name);
        partEntity.AddComponent(new SpriteRenderer(new Sprite(_editor.SpriteSheet.Texture, part.SourceSprite.Region)));
        part.Transform.Apply(partEntity.Transform);
        partEntity.Transform.SetParent(mainEntity.Transform);
        _entities.Add(partEntity);
      }
      mainEntity.SetParent(this);
      _main = mainEntity;
      // mainEntity.Transform.Scale = new Vector2(2f, 2f); 
    }
    public void UnEdit()
    {
      RemoveAllComponents();
      foreach (var entity in _entities) entity.Destroy();
      _editor.Set(Editor.EditingState.Default);
      _editor.GetComponent<SheetView>().IsCollapsed = false;
    }
    public override void Update()
    {
      base.Update();
      if (_entities.Count == 0) return;
      if (_gui.Selection is Sprites.Spritex complex)
      {     
      }
      SelectInput();
    }
    void SelectInput()
    {

    }
  }
}
