using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;
using Num = System.Numerics;


namespace Tools 
{
  public partial class SpriteSheetEditor 
  {
    public partial class SheetImageControl : Control 
    {
      public SheetImageControl()
      {
      }
      public override void Render() 
      {
        if (ImGui.Begin(Names.ContentWindow, ImGuiWindowFlags.MenuBar))
        { 
          var frame = ImGui.GetStyle().FramePadding.X + ImGui.GetStyle().FrameBorderSize;
          ImGui.BeginChild("spritesheet-view", new Num.Vector2(ImGui.GetContentRegionAvail().X - 0, 0), false, 
              ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        
          RenderUpdate();

          // clamp in such a way that we keep some part of the image visible
          var min = -(Editor.SpriteSheet.Size.ToNumerics() * Gui.ContentZoom) * 0.8f;
          var max = ImGui.GetContentRegionAvail() * 0.8f;
          Gui.SheetPosition = Num.Vector2.Clamp(Gui.SheetPosition, min, max);
          ImGui.SetCursorPos(Gui.SheetPosition);

          var cursorPosImageTopLeft = ImGui.GetCursorScreenPos();
          ImGui.Image(Gui.SheetTexture, Editor.SpriteSheet.Size.ToNumerics() * Gui.ContentZoom);
          ImGui.EndChild();

          DrawSpritesRegion();
          DrawMenubar();
          ImGui.End();
        }
      }     
      void RenderUpdate()
      {
        // if (ImGui.IsWindowHovered() && Editor.EditState == EditingState.INACTIVE) Editor.Set(EditingState.ACTIVE);
        if (Gui.Selection == null) Gui.SelectionRect = new SelectionRectangle(new RectangleF(), Gui);  
        Gui.SelectionRect.DrawWithInput(Gui, Editor);
        if (Gui.SelectionRect != null && Gui.SelectionRect.IsEditingPoint) 
        {
          Gui.SelectionRect.Snap(Editor.TileWidth, Editor.TileHeight);
          Gui.SelectionRect.Update();
        }

        if (ImUtils.HasMouseRealClickAt(ImUtils.GetWindowRect()) && Editor.EditState != EditingState.AutoRegion) 
        {
          Editor.Set(EditingState.Default);
        }
        if (Editor.EditState == EditingState.Default && ImGui.IsWindowHovered())
        {
          var (windowMin, windowMax) = ImUtils.GetWindowArea();
          ImUtils.DrawRealRect(ImGui.GetWindowDrawList(), ImUtils.GetWindowRect(), Editor.ColorSet.ContentActiveOutline);

          ZoomInput();
          MoveInput();
          SelectInput();  

        }
      }
      void DrawSpritesRegion() 
      {
        if (Editor.SpriteSheet == null) return;
        foreach (var (id, tile) in Editor.SpriteSheet.Tiles)
        {
          ImUtils.DrawRect(ImGui.GetWindowDrawList(), tile.Region, Editor.ColorSet.SpriteRegionInactiveOutline, Gui.SheetPosition, Gui.ContentZoom);
        }
        if (Gui.Selection is TiledSpriteData tiledSprite)
        {
          ImUtils.DrawRectFilled(ImGui.GetWindowDrawList(), tiledSprite.Region, Editor.ColorSet.SpriteRegionActiveFill, Gui.SheetPosition, Gui.ContentZoom);
          ImUtils.DrawRect(ImGui.GetWindowDrawList(), tiledSprite.Region, Editor.ColorSet.SpriteRegionActiveOutline, Gui.SheetPosition, Gui.ContentZoom);
        }

      }
      void ZoomInput()
      {
        if (ImGui.GetIO().MouseWheel != 0) 
        {
          var minZoom = 0.2f;
          var maxZoom = 10f;

          var oldSize = Gui.ContentZoom * Editor.SpriteSheet.Size;
          var zoomSpeed = Gui.ContentZoom * 0.2f;
          Gui.ContentZoom += Math.Min(maxZoom - Gui.ContentZoom, ImGui.GetIO().MouseWheel * zoomSpeed);
          Gui.ContentZoom = Mathf.Clamp(Gui.ContentZoom, minZoom, maxZoom);

          // zoom in, move up/left, zoom out the opposite
          var deltaSize = oldSize - (Gui.ContentZoom * Editor.SpriteSheet.Size);
          Gui.SheetPosition += deltaSize.ToNumerics() * 0.5f;
        }

      }
      Vector2 _initialSheetPosition = new Vector2();
      void MoveInput()
      {
        if (Gui.IsDragFirst)
        {
          _initialSheetPosition = Gui.SheetPosition;
        }
        if (Gui.IsDrag && Gui.MouseDragButton == 2) 
        {
          ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
          Gui.SheetPosition.X = _initialSheetPosition.X - (Gui.MouseDragStart.X - ImGui.GetIO().MousePos.X);
          Gui.SheetPosition.Y = _initialSheetPosition.Y - (Gui.MouseDragStart.Y - ImGui.GetIO().MousePos.Y);
        } 
        else ImGui.SetMouseCursor(ImGuiMouseCursor.Arrow);
      }
      void SelectInput()
      {
        if (ImGui.GetIO().MouseClicked[0] || ImGui.GetIO().MouseClicked[1])
        {
          if (Gui.Selection == null) 
          {
            foreach (var (tileId, tileData) in Editor.SpriteSheet.Tiles)
            {
              if (ImUtils.HasMouseClickAt(tileData.Region.ToRectangleF(), Gui.ContentZoom, Gui.SheetPosition)) Select(tileData);
            }
          }
          else if (!Gui.SelectionRect.IsEditingPoint) 
          {
            Gui.Selection = null;
            Editor.Set(EditingState.Default);
          }
        }
      }
      public void Select(object sel)
      { 
        Gui.Selection = sel;
        if (Gui.Selection is TiledSpriteData tileData)
        {
          Gui.SelectionRect = new SelectionRectangle(tileData.Region.ToRectangleF(), Gui);
        }
        else if (Gui.Selection is ComplexSpriteData complex)
        {

        }
        Editor.Set(EditingState.SelectedSprite);
      }
    }
  }
}
