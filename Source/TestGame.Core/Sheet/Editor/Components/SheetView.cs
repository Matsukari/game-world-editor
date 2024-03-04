using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace Raven.Sheet
{
  public class SheetView : Editor.SubEntity
  {
    public bool IsCollapsed = false;
    public bool IsSpritesView = false;
    SpriteRenderer _image;
    public override void OnAddedToScene()
    {
      _image = AddComponent(new SpriteRenderer(Gui.SheetTexture));
      LocalScale = Screen.Size / Gui.SheetTexture.GetSize();
      var min = Math.Min(LocalScale.X, LocalScale.Y);
      LocalScale = new Vector2(min, min);
      AddComponent(new Renderable());
    }    
    public class Renderable : Editor.SubEntity.RenderableComponent<SheetView>
    {
      public override void Render(Batcher batcher, Camera camera)
      {
        if (Editor.SpriteSheet == null) return;
        if (Gui.Selection is Sprites.Sprite sprite)
        {
          batcher.DrawRect(Entity.Position+sprite.Region.Location.ToVector2(), 
              sprite.Region.Size.X * Entity.Scale.X, 
              sprite.Region.Size.Y * Entity.Scale.Y, 
              Editor.ColorSet.SpriteRegionActiveFill);
        }
      }
    }
        
    public override void OnEditorUpdate()
    {
      if (ImGui.IsKeyReleased(ImGuiKey.Escape) && IsCollapsed)
      {
        Editor.GetSubEntity<SpritexView>().UnEdit();
      }
      else if (ImUtils.IsMouseAt(ImUtils.GetWindowRect()) && Editor.EditState != Editor.EditingState.AutoRegion && !IsCollapsed)
      {
        Editor.Set(Editor.EditingState.Default);
      }

      if (IsCollapsed) return;
      // if (ImGui.IsWindowHovered() && Editor.EditState == EditingState.INACTIVE) Editor.Set(EditingState.ACTIVE);
      if (Gui.Selection == null) Gui.SelectionRect.Enabled = false;
      if (Gui.SelectionRect != null && Gui.SelectionRect.IsEditingPoint) 
      {
        Gui.SelectionRect.Enabled = true;
        Gui.SelectionRect.Ren.Snap(Editor.TileWidth, Editor.TileHeight);
        Gui.SelectionRect.Update();
      }

      if (Editor.EditState == Editor.EditingState.Default )
      {
        var (windowMin, windowMax) = ImUtils.GetWindowArea();
        ImUtils.DrawRealRect(ImGui.GetWindowDrawList(), ImUtils.GetWindowRect(), Editor.ColorSet.ContentActiveOutline);

        ZoomInput();
        MoveInput();
        SelectInput();  

      }
    }
    void ZoomInput()
    {
      if (ImGui.GetIO().MouseWheel != 0) 
      {
        var minZoom = 0.4f;
        var maxZoom = 5f;
        float zoomFactor = 1.2f;
        if (ImGui.GetIO().MouseWheel < 0) zoomFactor = 1/zoomFactor;
        var zoom = Scene.Camera.RawZoom;
        var delta = (ImGui.GetIO().MousePos - Scene.Camera.Position) * (zoomFactor - 1);
        zoom = Math.Clamp(zoom * zoomFactor, minZoom, maxZoom);
        Position -= delta;
        Scene.Camera.RawZoom = zoom;
      }
    }
    Vector2 _initialSheetPosition = new Vector2();
    void MoveInput()
    {
      if (Gui.IsDragFirst)
      {
        _initialSheetPosition = Scene.Camera.Position;
      }
      if (Gui.IsDrag && Gui.MouseDragButton == 2) 
      {
        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        Scene.Camera.Position = _initialSheetPosition + (Gui.MouseDragStart - ImGui.GetIO().MousePos);
      } 
      else ImGui.SetMouseCursor(ImGuiMouseCursor.Arrow);
    }
    void SelectInput()
    {
      if (ImGui.GetIO().MouseClicked[0] || ImGui.GetIO().MouseClicked[1])
      {
        if (Gui.Selection == null && IsSpritesView) 
        {
          foreach (var sprite in Editor.SpriteSheet.Sprites)
          {
            if (ImUtils.HasMouseClickAt(sprite.Value.Region.ToRectangleF())) Select(sprite);
          }
        }
        else if (!Gui.SelectionRect.IsEditingPoint) 
        {
          Gui.Selection = null;
          Editor.Set(Editor.EditingState.Default);
        }
      }
    }
    public void Select(object sel)
    {
      if (Gui.ShapeSelection != null) return;
      Gui.Selection = sel;
      if (Gui.Selection is Sprites.Sprite sprite)
      {
        Gui.SelectionRect = new Selection(Gui);
        Gui.SelectionRect.Ren.SetBounds(sprite.Region.ToRectangleF()); 
      }
      else if (Gui.Selection is Sprites.Spritex spritex)
      {

      }
      Editor.Set(Editor.EditingState.SelectedSprite);
    }
  }
}
