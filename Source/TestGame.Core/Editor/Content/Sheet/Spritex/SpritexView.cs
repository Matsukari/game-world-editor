using Nez;
using ImGuiNET;
using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace Raven
{
  // <summary>
  // Handles state changes between sheet and spriteScene 
  // </summary>
  public class SpriteSceneView : EditorComponent, IImGuiRenderable
  {
    public SpriteSceneInspector LastSprite { get => _spriteScene; }
    SpriteSceneInspector _spriteScene;
    SpriteSceneSpritePicker _picker;

    public override void OnContent()
    {
      if (RestrictTo<Sheet>())
      {
        _picker = new SpriteSceneSpritePicker(this);
        Editor.GetEditorComponent<SheetView>()._inspector.SpritePicker.HandleSelectedSprite = _picker.HandleSelectedSprite;
      }
      Clean();
    }        
    // Go to canvas and close spritesheet view
    public void Edit(Sprites.SpriteScene spriteScene)
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
      _spriteScene = new SpriteSceneInspector(this, spriteScene);
      Enabled = true;
      ContentData.Selection = spriteScene;
      ContentData.ShapeContext = spriteScene;
      Editor.GetEditorComponent<SheetView>().Enabled = false;
      Editor.GetEditorComponent<SheetView>()._inspector.ShowPicker = true; 
  
      // Rsetore last state
      Entity.Scene.Camera.Position = _spriteScene.GuiPosition;
      Entity.Scene.Camera.Zoom = _spriteScene.GuiZoom;

      Entity.GetComponent<Utils.Components.CameraMoveComponent>().Enabled = true;
      Entity.GetComponent<Utils.Components.CameraZoomComponent>().Enabled = true;
      spriteScene.Entity = Entity;
    }

    // back to spritesheet view
    public void UnEdit()
    {
      Clean();
      Editor.GetEditorComponent<SheetView>().Enabled = true;
      Editor.GetEditorComponent<SheetView>()._inspector.ShowPicker = false; 
      Editor.GetEditorComponent<AnimationEditor>().Close();

      // Sotre last state
      _spriteScene.GuiPosition = Entity.Scene.Camera.Position;
      _spriteScene.GuiZoom = Entity.Scene.Camera.RawZoom;

      // Enter sheet vew
      Entity.Scene.Camera.RawZoom = ContentData.Zoom;
      Entity.Scene.Camera.Position = ContentData.Position;
    }
    void Clean()
    {
      if (!Editor.HasContent) return;
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
      Guidelines.OriginLinesRenderable.Render(batcher, camera, Editor.Settings.Colors.OriginLineX.ToColor(), Editor.Settings.Colors.OriginLineY.ToColor());
      _spriteScene.SpriteScene.Render(batcher, camera);
      foreach (var part in _spriteScene.SpriteScene.Parts)
      {
        batcher.DrawString(
            Graphics.Instance.BitmapFont, 
            part.Name,
            part.WorldBounds.BottomLeft(),
            color: Color.DarkGray, 
            rotation: 0f, 
            origin: Vector2.Zero, 
            scale: Math.Clamp(1f/camera.RawZoom, 1f, 10f), 
            effects: Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 
            layerDepth: 0f);
      }

    }  
    public void Render(Editor editor) 
    {

      if (Nez.Input.RightMouseButtonPressed)
      {
        // var hasSelection = false;
        foreach (var part in _spriteScene.SpriteScene.Parts)
        {
          var mouse = Entity.Scene.Camera.MouseToWorldPoint();
          if (part.WorldBounds.Contains(mouse))
          {
            // hasSelection = true;
          }
        }
        // if (!hasSelection)
        // {
        //   ImGui.OpenPopup("spriteScene-canvas-options-popup");
        // }
      }
      // if (ImGui.BeginPopup("spriteScene-canvas-options-popup"))
      // {
      //   if (ImGui.MenuItem("Add new component here"))
      //   {
      //   }
      //   ImGui.EndPopup();
      // }
      Editor.GetEditorComponent<SheetView>()._inspector.Render(editor);
      _spriteScene.Render(editor);

    }
    Vector2 _initialScale = new Vector2();
    void HandleSelection()
    {
      var selectionRect = Editor.GetEditorComponent<Selection>();

      // select individual parts
      if (Nez.Input.LeftMouseButtonPressed || Nez.Input.RightMouseButtonPressed)
      {
        foreach (var part in _spriteScene.SpriteScene.Parts)
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
