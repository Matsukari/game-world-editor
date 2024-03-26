using Nez;
using ImGuiNET;
using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace Raven.Sheet
{
  // <summary>
  // Handles state changes between sheet and spritex 
  // </summary>
  public class SpritexView : EditorComponent, IImGuiRenderable
  {
    public SpritexInspector LastSprite { get => _spritex; }
    SpritexInspector _spritex;

    public override void OnAddedToEntity()
    {
      _entityHandle = Entity.Scene.CreateEntity("editor-spritex");
    }
        
    public override void OnContent()
    {
      RestrictTo<Sheet>();
      Clean();
    }        
    // Go to canvas and close spritesheet view
    public void Edit(Sprites.Spritex spritex)
    {
      // came from sheet
      if (!Enabled)
      {
        // Save last sheet view state 
        ContentData.Position = Entity.Scene.Camera.Position;
        ContentData.Zoom = Entity.Scene.Camera.RawZoom;
      }

      Clean();

      // Prepare
      _spritex = new SpritexInspector(this, spritex);
      Enabled = true;
      ContentData.Selection = spritex;
      ContentData.ShapeContext = spritex;
      Editor.GetEditorComponent<SheetView>().Enabled = false;
  
      // Rsetore last state
      Entity.Scene.Camera.Position = _spritex.GuiPosition;
      Entity.Scene.Camera.Zoom = _spritex.GuiZoom;

      Entity.GetComponent<Utils.Components.CameraMoveComponent>().Enabled = true;
      Entity.GetComponent<Utils.Components.CameraZoomComponent>().Enabled = true;
      _entityHandle.SetParent(Entity);
      _spritexComponent = _entityHandle.AddComponent(spritex);
    }
    Component _spritexComponent;
    Entity _entityHandle;

    // back to spritesheet view
    public void UnEdit()
    {
      Clean();
      Editor.GetEditorComponent<SheetView>().Enabled = true;

      // Sotre last state
      _spritex.GuiPosition = Entity.Scene.Camera.Position;
      _spritex.GuiZoom = Entity.Scene.Camera.RawZoom;

      // Enter sheet vew
      Entity.Scene.Camera.RawZoom = ContentData.Zoom;
      Entity.Scene.Camera.Position = ContentData.Position;
    }
    void Clean()
    {
      if (_entityHandle != null)
      {
        _entityHandle.RemoveAllComponents();
      }
      Editor.GetEditorComponent<SheetSelector>().RemoveSelection();
      Editor.GetEditorComponent<Selection>().End();
      Enabled = false;
      ContentData.ShapeContext = Content;
      ContentData.Selection = null;
    }

    public override void Update()
    {
      base.Update();
      if (Nez.Input.IsKeyReleased(Keys.Escape)) UnEdit(); 
      HandleSelection();
    }
    public override void Render(Batcher batcher, Camera camera)
    {
      Guidelines.OriginLinesRenderable.Render(batcher, camera, Editor.Settings.Colors.OriginLineX, Editor.Settings.Colors.OriginLineY);
    }  
    public void Render(Editor editor) 
    {
      Editor.GetEditorComponent<SheetView>()._inspector.Render(editor);
      _spritex.Render(editor);

      if (Nez.Input.RightMouseButtonPressed)
      {
        var hasSelection = false;
        foreach (var part in _spritex.Spritex.Body)
        {
          var mouse = Entity.Scene.Camera.MouseToWorldPoint();
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
      if (ImGui.BeginPopup("spritex-canvas-options-popup"))
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
      var selectionRect = Editor.GetEditorComponent<Selection>();

      // select individual parts
      if (Nez.Input.LeftMouseButtonPressed || Nez.Input.RightMouseButtonPressed)
      {
        foreach (var part in _spritex.Spritex.Body)
        {
          var mouse = Entity.Scene.Camera.MouseToWorldPoint();
          if (part.WorldBounds.Contains(mouse))
          {
            if (Nez.Input.LeftMouseButtonPressed)
            {
              _initialScale = part.Transform.Scale;
              selectionRect.Begin(part.WorldBounds, part);
              return;
            }
          }
        }
      }
      if (selectionRect.Capture is SourcedSprite selPart)
      {
        selPart.Transform.Scale = _initialScale + 
          (selectionRect.ContentBounds.Size - selectionRect.InitialBounds.Size) / (selPart.SourceSprite.Region.Size.ToVector2());
        selPart.Transform.Position = selectionRect.ContentBounds.Location + (selPart.Origin * selPart.Transform.Scale);
      }
    }
  }
}