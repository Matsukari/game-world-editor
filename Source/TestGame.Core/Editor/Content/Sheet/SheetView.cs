using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace Raven
{
  public class SheetView : ContentView
  {
    public override IImGuiRenderable ImGuiHandler => _imgui;
    public override IInputHandler InputHandler => _input;
    public Sheet Sheet { get => _sheet; }
 
    SheetViewImGui _imgui;
    SheetViewInputHandler _input;

    Sheet _sheet;
    SpriteRenderer _image;
    List<Rectangle> _tiles;
    SpriteSceneView _scene;
    
    public Rectangle TileInMouse;

    public SheetInspector Inspector { get => _imgui.Inspector; }
    public SpriteAnimationEditor SpriteAnimationEditor { get => _imgui.SpriteAnimEditor; }

    public override bool CanDealWithType(object content) => content is Sheet;

    public override void Initialize(Editor editor, EditorContent content)
    {
      base.Initialize(editor, content);

      _scene = new SpriteSceneView(this);
      _scene.Initialize(editor, content);
      _scene.OnEdit += () => Inspector.ShowPicker = true;
      _scene.OnEdit += () => _imgui.SpriteAnimEditor.Close();
      _scene.OnUnEdit += () => Inspector.ShowPicker = false;
      _scene.OnUnEdit += () => _imgui.SpriteAnimEditor.Close();
      _scene.OnUnEdit += () => ContentData.PropertiedContext = _sheet;

      _imgui = new SheetViewImGui(Settings, Camera);
      _imgui.Initialize(editor, content);
      _imgui.Update(_sheet, ContentData.SelectionList);
      _imgui.SceneView = _scene;

      _imgui.Popups.Initialize(editor, content);
      _imgui.Popups.OnConvertToScene += scene => _scene.Edit(scene);
      _imgui.SpriteAnimEditor.Initialize(editor, content);

      _input = new SheetViewInputHandler(this);
      _input.Initialize(editor, content);
      _input.OnSelectionRightClick += () => 
      { 
        if (ContentData.SelectionList.Last() is Sprite) _imgui.Popups.OpenSpriteOptions(); 
        else _imgui.Popups.OpenTileOptions(); 
      };

      Inspector.OnClickScene += scene => _scene.Edit(scene);
      Inspector.OnDeleteScene += scene => _scene.UnEdit();


    }
    public override void OnContentOpen(IPropertied content)
    {
      TileInMouse = Rectangle.Empty;
      _sheet = content as Sheet;
      _image = new SpriteRenderer(_sheet.Texture);
      _image.Entity = Entity;
    }
    public override void OnContentClose()
    {
      Selection.End();
    }
        
    public override void Render(Batcher batcher, Camera camera, EditorSettings settings)
    {
      _imgui.Update(_sheet, ContentData.SelectionList);

      if (_scene.IsEditing) 
      {
        _scene.Render(batcher, camera);
        return;
      }

      _image.Render(batcher, camera);

      TileInMouse = Rectangle.Empty;


      
      // Draw tiles grid
      for (int x = 0; x < _sheet.Tiles.X; x++)
      {
        for (int y = 0; y < _sheet.Tiles.Y; y++)
        {
          var worldTile = Sheet.GetTile(x, y).ToRectangleF();
          worldTile.Location += _image.Bounds.Location;
          if (worldTile.Contains(camera.MouseToWorldPoint()) ) TileInMouse = Sheet.GetTile(x, y);
          if (Settings.Graphics.DrawSheetGrid)
          {
            batcher.DrawRectOutline(camera, worldTile, settings.Colors.PickHoverOutline.ToColor());
          }

        }
      }

      // Highlight the tile under mouse
      if (!TileInMouse.IsEmpty && !_input.IsBlocked && ContentData.SelectionList.Selections.Count() <= 1) 
      {
        var worldTileInMouse = TileInMouse.ToRectangleF();
        worldTileInMouse.Location += _image.Bounds.Location;
        batcher.DrawRect(worldTileInMouse, settings.Colors.PickHover.ToColor());
      }

      if (_imgui.Popups.SpriteSlicer.IsOpen)
      { 
        Raven.Guidelines.GridLines.RenderGridLines(batcher, camera, GetRegionInSheet(_imgui.Popups.SpriteSlicer.Sprite.Region.ToRectangleF()).Location, 
            settings.Colors.PickSelectedOutline.ToColor(), _imgui.Popups.SpriteSlicer.SplitCount, _imgui.Popups.SpriteSlicer.SplitSize.ToVector2());
      }

      if (_imgui.SpriteAnimEditor.IsOpen) 
      {
        for (int i = 0; i < _imgui.SpriteAnimEditor.Player.Animation.Frames.Count(); i++)
        {
          if (_imgui.SpriteAnimEditor.Player.Animation.Frames[i] is not SpriteAnimationFrame spriteFrame) return;

          batcher.DrawRect(GetRegionInSheet(spriteFrame.Sprite.Region.ToRectangleF()), settings.Colors.Background.ToColor());
          if (_imgui.SpriteAnimEditor.Player.CurrentIndex == i)
            batcher.Draw(
                texture: Sheet.Texture, 
                sourceRectangle: spriteFrame.Sprite.Region, 
                destinationRectangle: GetRegionInSheet((_imgui.SpriteAnimEditor.Animation.Frames[0] as SpriteAnimationFrame).Sprite.Region.ToRectangleF()),
                color: Color.White);
        }
      }

      // Draw last selected sprite
      foreach (var selection in ContentData.SelectionList.Selections)
      {
        RectangleF region = RectangleF.Empty;
        if (selection is Sprite sprite) region = sprite.Region.ToRectangleF();
        else if (selection is Tile tile) region = tile.Region.ToRectangleF();

        if (region != RectangleF.Empty) 
        {
          batcher.DrawRect(GetRegionInSheet(region), settings.Colors.PickFill.ToColor());
          batcher.DrawRectOutline(camera, GetRegionInSheet(region), settings.Colors.PickSelectedOutline.ToColor());
        }

      }
    }

    public RectangleF GetRegionInSheet(RectangleF rectangle)
    {
      rectangle.Location += _image.Bounds.Location;
      return rectangle;
    }
    public Vector2 GetOffRegionInSheet(Vector2 pos)
    {
      pos -= _image.Bounds.Location;
      return pos;
    }
  }
}
