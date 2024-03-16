using Nez;
using ImGuiNET;
using Nez.Sprites;
using Nez.Textures;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace Raven.Sheet
{
  public class SpritexView : Editor.SheetEntity
  {
    public Sprites.Spritex LastSprite { get => _spritex; }
    Sprites.Spritex _spritex;
    List<Entity> _entities = new List<Entity>();

    public override void OnChangedTab()
    {
      Clean();
    }
    public override void OnDisableTab()
    {
      Clean();
    }        
    // Go to canvas and close spritesheet view
    public void Edit(Sprites.Spritex spritex)
    {
      // came from sheet
      if (!Enabled)
      {
        // Save last sheet view state 
        Gui.Position = Scene.Camera.Position;
        Gui.Zoom = Scene.Camera.RawZoom;
      }


      Clean();

      // Prepare
      _spritex = spritex;
      Enabled = true;
      Gui.Selection = _spritex;
      Gui.ShapeContext = _spritex;
      Editor.GetSubEntity<SheetView>().Enabled = false;
      Editor.Set(Editor.EditingState.SelectedSprite);
  
      // Rsetore last state
      Scene.Camera.Position = _spritex.GuiPosition;
      Scene.Camera.Zoom = _spritex.GuiZoom;

      // Origin lines
      var origin = AddComponent(new Guidelines.OriginLines());
      origin.Color = Editor.ColorSet.SpriteRegionActiveOutline;

      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent());

      BuildBody();
    }
    // back to spritesheet view
    public void UnEdit()
    {
      Clean();

      // Sotre last state
      _spritex.GuiPosition = Scene.Camera.Position;
      _spritex.GuiZoom = Scene.Camera.RawZoom;

      // Enter sheet vew
      Scene.Camera.RawZoom = Gui.Zoom;
      Scene.Camera.Position = Gui.Position;
    }
    void Clean()
    {
      RemoveAllComponents();
      foreach (var entity in _entities) entity.Destroy();
      _entities.Clear();
      Position = Screen.Center;
      Editor.GetSubEntity<SheetView>().Enabled = true;
      Editor.Set(Editor.EditingState.Default);
      Editor.GetSubEntity<SheetSelector>().RemoveSelection();
      Editor.GetSubEntity<Selection>().End();
      Enabled = false;
      Gui.ShapeContext = Sheet;
    }

    public override void Update()
    {
      base.Update();
      if (Nez.Input.IsKeyReleased(Keys.Escape)) UnEdit(); 

      SyncEntitySpritex();
      HandleSelection();
    }
    // Recreate and sync entities to spritex parts
    void BuildBody()
    {
      foreach (var entity in _entities) entity.Destroy();
      _entities.Clear();
      foreach (var part in _spritex.Body)
      {
        var partEntity = Scene.CreateEntity(_spritex.Name + part.SourceSprite.Name);
        var ren = partEntity.AddComponent(new SpriteRenderer(new Sprite(Sheet.Texture, part.SourceSprite.Region)));
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

      // select individual parts
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
              Gui.Selection = _spritex;
              selectionRect.Begin(part.Bounds, part);
              return;
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
  }
}
