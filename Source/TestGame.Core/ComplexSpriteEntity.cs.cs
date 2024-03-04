using Nez;
using ImGuiNET;
using Nez.Sprites;
using Nez.Textures;
using Microsoft.Xna.Framework;


namespace Tools 
{
  public partial class SpriteSheetEditor : Component
  {
    public class ComplexSpriteEntity : Entity
    {
      GuiData _gui;
      SpriteSheetEditor _editor;
      ComplexSpriteData _sprite;
      List<Entity> _entities = new List<Entity>();
      float _zoom = 1;
      public Entity _main;
      public ComplexSpriteEntity(GuiData gui, SpriteSheetEditor editor)
      {
        _gui = gui;
        _editor = editor;
        Name = Names.ComplexSprite;;
      }
      public void Edit(ComplexSpriteData sprite)
      {
        Position = Screen.Center;
        UnEdit();
        _sprite = sprite;
        _editor.GetComponent<SheetImageControl>().IsCollapsed = true;
        _editor.Set(EditingState.SelectedSprite);
        // var background = AddComponent(new PrototypeSpriteRenderer());
        // background.Color = _editor.ColorSet.Background * 0.5f;
        var origin = AddComponent(new OriginLinesRenderable());
        origin.Color = _editor.ColorSet.SpriteRegionActiveOutline;

        var mainEntity = Scene.CreateEntity(_sprite.Name);
        foreach (var part in _sprite.GetFullBody())
        {
          var partEntity = Scene.CreateEntity(_sprite.Name + part.Tile.Name);
          partEntity.AddComponent(new SpriteRenderer(new Sprite(_editor.SpriteSheet.Texture, part.Tile.Region)));
          part.LocalState.Apply(partEntity.Transform);
          partEntity.Transform.SetParent(mainEntity.Transform);
          _entities.Add(partEntity);
        }
        mainEntity.SetParent(this);
        _main = mainEntity;
        // mainEntity.Transform.Scale = new Vector2(2f, 2f); 
      }
      public void UnEdit()
      {
        RemoveAllComponents();
        foreach (var entity in _entities) entity.Destroy();
        _editor.Set(EditingState.Default);
        _editor.GetComponent<SheetImageControl>().IsCollapsed = false;
      }
      public void SheetUpdate()
      {
        // Console.WriteLine(Scene.Camera.Bounds.ToString());
        if (_entities.Count == 0) return;
        // _entities.First().LocalPosition = _sprite.BodyOrigin.LocalState.Position;
        // _entities.First().LocalRotationDegrees = _sprite.BodyOrigin.LocalState.Rotation;
        // _entities.First().LocalScale = _sprite.BodyOrigin.LocalState.Scale;

        if (_gui.Selection is ComplexSpriteData complex)
        {     
        }
        ZoomInput();
        MoveInput();
        SelectInput();
        DrawPropertiesPane();
      }
      void ZoomInput()
      {
        if (ImGui.GetIO().MouseWheel != 0)
        {
          var minZoom = 0.4f;
          var maxZoom = 5f;

          float zoomFactor = 1.2f;
          if (ImGui.GetIO().MouseWheel < 0) zoomFactor = 1/zoomFactor;
          var delta = (ImGui.GetIO().MousePos - Position) * (zoomFactor - 1);
          _zoom = Math.Clamp(_zoom * zoomFactor, minZoom, maxZoom);
          Position -= delta;
          Scale = new Vector2(_zoom, _zoom);
          Console.WriteLine($"delta: {delta}");

        }
      }
      Vector2 _initialCameraPosition = new Vector2();
      void MoveInput()
      {
        Console.WriteLine($"{Scene.Camera.Zoom}");
        if (_gui.IsDragFirst)
        {
          _initialCameraPosition = Position;
        }
        if (_gui.IsDrag && _gui.MouseDragButton == 2) 
        {
          Position = _initialCameraPosition - (_gui.MouseDragStart - ImGui.GetIO().MousePos);
        } 
      }
      void SelectInput()
      {

      }
      void DrawPropertiesPane()
      {
        ImGui.Begin(Names.ObjectPropertiesPane);
        ImGui.Indent();
        var name = _sprite.Name; 
        if (ImGui.InputText("Name", ref name, 12)) _sprite.Name = name;
        ImGui.NewLine();
        if (ImGui.MenuItem("Animation")) 
        {

        }
        ImGui.NewLine();
        ImGui.SeparatorText("Parts");
        ImGui.NewLine();
        ImGui.Indent();
        if (ImGui.MenuItem(_sprite.BodyOrigin.Tile.Name))
        {
        }
        foreach (var part in _sprite.Body.Parts)
        {
          if (ImGui.MenuItem(part.Key))
          {
          }
        }
        ImGui.Unindent();
        ImGui.NewLine();
        ImGui.PushStyleColor(ImGuiCol.Button, _editor.ColorSet.DeleteButton.ToImColor());
        if (ImGui.Button("Delete")) 
        {
        }
        ImGui.PopStyleColor();
        ImGui.Unindent();
        ImGui.NewLine();
        ImGui.Separator();
        ImGui.NewLine();
        SheetPropertiesControl.DrawCustomProperties(_sprite.Properties, _editor);
        ImGui.End();
      }
    }
  }
}
