using Nez;
using ImGuiNET;
using Nez.Sprites;
using Nez.Textures;
using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Nez.ImGuiTools;


namespace Raven.Sheet
{
  public class SpritexView : Editor.SheetEntity
  {
    public SpritexGui LastSprite { get => _spritex; }
    public override void OnAddedToScene() => Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);
        
    SpritexGui _spritex;


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
      _spritex = new SpritexGui(spritex);
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
      var x = AddComponent(_spritex.Spritex);

    }
    // back to spritesheet view
    public void UnEdit()
    {
      Clean();
      Editor.GetSubEntity<SheetView>().Enabled = true;

      // Sotre last state
      _spritex.GuiPosition = Scene.Camera.Position;
      _spritex.GuiZoom = Scene.Camera.RawZoom;

      // Enter sheet vew
      Scene.Camera.RawZoom = Gui.Zoom;
      Scene.Camera.Position = Gui.Position;
    }
    void Clean()
    {
      Components.RemoveAllComponents();
      Editor.Set(Editor.EditingState.Default);
      Editor.GetSubEntity<SheetSelector>().RemoveSelection();
      Editor.GetSubEntity<Selection>().End();
      if (Enabled) Enabled = false;
      Gui.ShapeContext = SheetGui;
    }

    public override void Update()
    {
      base.Update();
      if (Nez.Input.IsKeyReleased(Keys.Escape)) UnEdit(); 
      HandleSelection();
    }
    public void RenderImGui() 
    {
      if (!Enabled) return;
      if (Nez.Input.RightMouseButtonPressed)
      {
        var hasSelection = false;
        foreach (var part in _spritex.Spritex.Body)
        {
          var mouse = Scene.Camera.MouseToWorldPoint();
          if (part.WorldBounds.Contains(mouse))
          {
            hasSelection = true;
          }
        }
        if (!hasSelection)
        {
          ImGui.OpenPopup("spritex-canvas-options-popup");
        }
      }
      if (ImGui.BeginPopupContextItem("spritex-canvas-options-popup"))
      {
        if (ImGui.MenuItem("Add new component here"))
        {
        }
        ImGui.EndPopup();
      }
    }
    Vector2 _initialScale = new Vector2();
    void HandleSelection()
    {
      var selectionRect = Editor.GetSubEntity<Selection>();

      // select individual parts
      if (Nez.Input.LeftMouseButtonPressed || Nez.Input.RightMouseButtonPressed)
      {
        foreach (var part in _spritex.Spritex.Body)
        {
          var mouse = Scene.Camera.MouseToWorldPoint();
          if (part.WorldBounds.Contains(mouse))
          {
            if (Nez.Input.LeftMouseButtonPressed)
            {
              _initialScale = part.Transform.Scale;
              Gui.Selection = _spritex;
              selectionRect.Begin(part.WorldBounds, part);
              return;
            }
          }
        }
      }
      if (selectionRect.Capture is SourcedSprite selPart)
      {
        selPart.Transform.Scale = _initialScale + (selectionRect.Bounds.Size - selectionRect.InitialBounds.Size) / (selPart.SourceSprite.Region.Size.ToVector2());
        selPart.Transform.Position = selectionRect.Bounds.Location + (selPart.Origin * selPart.Transform.Scale);
      }
    }
  }
}
