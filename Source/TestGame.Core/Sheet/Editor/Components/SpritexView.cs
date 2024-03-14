using Nez;
using ImGuiNET;
using Nez.Sprites;
using Nez.Textures;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace Raven.Sheet
{
  public class SpritexView : Editor.SubEntity
  {
    public Sprites.Spritex LastSprite { get => _spritex; }
    Sprites.Spritex _spritex;
    List<Entity> _entities = new List<Entity>();
    public override void OnAddedToScene()
    {
      UnEdit();
    }
    public void Edit(Sprites.Spritex spritex)
    {
      UnEdit();
      _spritex = spritex;
      Position = new Vector2();
      Enabled = true;
      Gui.Selection = _spritex;
      Gui.ShapeContext = _spritex;
      Editor.GetSubEntity<SheetView>().Enabled = false;
      Editor.Set(Editor.EditingState.SelectedSprite);

      var origin = AddComponent(new Guidelines.OriginLines());
      origin.Color = Editor.ColorSet.SpriteRegionActiveOutline;

      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent());

      BuildBody();
    }
    public void UnEdit()
    {
      RemoveAllComponents();
      foreach (var entity in _entities) entity.Destroy();
      _entities.Clear();
      Position = Screen.Center;
      Editor.GetSubEntity<SheetView>().Enabled = true;
      Editor.Set(Editor.EditingState.Default);
      Enabled = false;
      Gui.ShapeContext = Editor.SpriteSheet;
      Scene.Camera.RawZoom = 1f;
      Scene.Camera.Position = new Vector2();
    }
    public override void Update()
    {
      base.Update();
      if (Nez.Input.IsKeyReleased(Keys.Escape)) UnEdit(); 

      SyncEntitySpritex();
      HandleSelection();
    }
    void BuildBody()
    {
      foreach (var entity in _entities) entity.Destroy();
      _entities.Clear();
      foreach (var part in _spritex.Body)
      {
        var partEntity = Scene.CreateEntity(_spritex.Name + part.SourceSprite.Name);
        var ren = partEntity.AddComponent(new SpriteRenderer(new Sprite(Editor.SpriteSheet.Texture, part.SourceSprite.Region)));
        part.Origin = ren.Origin;
        partEntity.SetParent(this);
        _entities.Add(partEntity);
      }
    }
    void SyncEntitySpritex()
    {
      if (_entities.Count() != _spritex.Body.Count() && Enabled)
      {
        BuildBody();
      }
      // Overall transform; which covers all parts
      // NOTE; it's the entity's transform that is begin modifed and synced to that of custom class' transform  and not the other way around
      _spritex.Transform.Apply(Transform);
      for (int i = 0; i < _entities.Count(); i++)
      {
        _spritex.Body[i].Transform.Apply(_entities[i].Transform);
      }
    }
    Vector2 _initialScale = new Vector2();
    void HandleSelection()
    {
      var selectionRect = Editor.GetSubEntity<Selection>();
      if (Nez.Input.LeftMouseButtonPressed || Nez.Input.RightMouseButtonPressed)
      {
        foreach (var part in _spritex.Body)
        {
          var mouse = Scene.Camera.MouseToWorldPoint();
          if (part.Bounds.Contains(mouse))
          {
            if (Nez.Input.LeftMouseButtonPressed)
            {
              _initialScale = part.Transform.Scale;
              selectionRect.Begin(part.Bounds, part);
              return;
            }
            else 
            {
              ImGui.OpenPopup("sprite-component-options");
            }
          }
        }
      }
      if (selectionRect.Capture is Sprites.Spritex.Sprite selPart)
      {
        selPart.Transform.Scale = _initialScale + (selectionRect.Bounds.Size - selectionRect.InitialBounds.Size) / (selPart.SourceSprite.Region.Size.ToVector2());
        selPart.Transform.Position = selectionRect.Bounds.Location + (selPart.Origin * selPart.Transform.Scale);
      }
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
