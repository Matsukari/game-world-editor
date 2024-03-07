using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using ImGuiNET;

namespace Raven.Sheet
{
  public class SheetSelector : Editor.SubEntity
  {
    public override void Update()
    {
      base.Update();
      var sheetView = Editor.GetSubEntity<SheetView>();
      if (Editor.EditState == Editor.EditingState.Default && sheetView.HasNoObstruction()) HandleInputs();
    }
    void HandleInputs()
    { 
      var sheetView = Editor.GetSubEntity<SheetView>();
      if (Nez.Input.LeftMouseButtonReleased || Nez.Input.RightMouseButtonReleased)
      {
        if (sheetView.IsSpritesView) 
        {
          foreach (var sprite in Editor.SpriteSheet.Sprites)
          {
            if (sheetView.GetRegionInSheet(sprite.Value.Region.ToRectangleF()).Contains(Scene.Camera.MouseToWorldPoint())) 
              Select(sprite.Value);
          }
        }
        else if (!sheetView.IsSpritesView && sheetView.TileInMouse != null) 
        {
          var coord = Editor.SpriteSheet.GetTile(sheetView.TileInMouse);
          var tileInCoord = Editor.SpriteSheet.CustomTileExists(coord.X, coord.Y);
          if (tileInCoord != null) Select(tileInCoord);
          else Select(new Sprites.Tile(coord, Editor.SpriteSheet));
        }
        else if (!Gui.SelectionRect.IsEditingPoint && Gui.Selection != null) 
        {
          Gui.Selection = null;
          Editor.Set(Editor.EditingState.Default);
        }
      }
    }

    public void Select(Shape shape)
    {
      Gui.ShapeSelection = shape;
    }
    public void Select(IPropertied sel)
    {
      if (Gui.ShapeSelection != null) return;
      Gui.Selection = sel;
      if (Gui.Selection is Sprites.Sprite sprite)
      {
        Gui.SelectionRect = new Selection(Gui);
        Gui.SelectionRect.Ren.SetBounds(sprite.Region.ToRectangleF()); 
      }
      else if (Gui.Selection is Sprites.Tile tile)
      {
      }
      else if (Gui.Selection is Sprites.Spritex spritex)
      {
      }
      else throw new TypeAccessException();
      Console.WriteLine($"Selected {Gui.Selection.GetType().Name}");
    }

  }
}

