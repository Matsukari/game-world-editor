using Nez;
using ImGuiNET;
using Nez.Sprites;
using Nez.Textures;
using Microsoft.Xna.Framework.Input;


namespace Raven.Sheet
{
  public class SpritexView : Editor.SubEntity
  {
    Entity _main;
    Sprites.Spritex _spritex;
    List<Entity> _entities = new List<Entity>();
    public void Edit(Sprites.Spritex spritex)
    {
      UnEdit();
      _spritex = spritex;
      Enabled = true;
      Gui.Selection = _spritex;
      Gui.ShapeContext = _spritex;
      Editor.GetSubEntity<SheetView>().Enabled = false;
      Editor.Set(Editor.EditingState.SelectedSprite);

      var origin = AddComponent(new Guidelines.OriginLines());
      origin.Color = Editor.ColorSet.SpriteRegionActiveOutline;
      // origin.RenderLayer = Editor.ScreenRenderLayer;

      var mainEntity = Scene.CreateEntity(_spritex.Name);
      foreach (var part in _spritex.Body)
      {
        var partEntity = Scene.CreateEntity(_spritex.Name + part.SourceSprite.Name);
        partEntity.AddComponent(new SpriteRenderer(new Sprite(Editor.SpriteSheet.Texture, part.SourceSprite.Region)));
        partEntity.SetParent(mainEntity);
        part.Transform.Apply(partEntity.Transform);
        _entities.Add(partEntity);
      }
      _main = mainEntity;
      _main.SetParent(this);
      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent());
    }
    public void UnEdit()
    {
      RemoveAllComponents();
      foreach (var entity in _entities) entity.Destroy();
      Editor.GetSubEntity<SheetView>().Enabled = true;
      Editor.Set(Editor.EditingState.Default);
      Enabled = false;
      Gui.ShapeContext = Editor.SpriteSheet;
      Scene.Camera.RawZoom = 1f;
    }
    public override void Update()
    {
      base.Update();
      if (_entities.Count == 0) return;
      if (Nez.Input.IsKeyReleased(Keys.Escape)) UnEdit(); 
    }
    internal bool HasNoObstruction()
    {
      return 
           !ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) 
        && !ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow)
        && Editor.EditState != Editor.EditingState.AnnotateShape;
    }
  }
}
