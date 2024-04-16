using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;
using Microsoft.Xna.Framework.Input;


namespace Raven.Widget
{
  public class AnnotatorPane : Window
  {
    static ShapeModel[] _shapeModels = new ShapeModel[]{new RectangleModel(), new EllipseModel(), new PointModel(), new PolygonModel()};
    public float Zoom = 1;
    public Vector2 Position = Vector2.Zero;
    public SourcedSprite SourcedSprite;
    Vector2 _initialPosition = Vector2.Zero;
    IPropertied _propertied;
    ShapeModel _shape;

    public AnnotatorPane() => _isOpen = false;

    public void Edit(SourcedSprite sprite, IPropertied propertied)
    {
      _propertied = propertied;
      SourcedSprite = sprite;
      _isOpen = true;
    }
    public void UnEdit()
    {
      _isOpen = false;
    }

    public override void OnRender(ImGuiWinManager imgui)
    {
      ImGuiUtils.DrawImage(ImGui.GetWindowDrawList(), SourcedSprite.SourceSprite.Texture, 
          SourcedSprite.SourceSprite.Region, 
          (SourcedSprite.Transform.Position + Position + ImGui.GetCursorScreenPos()).ToNumerics(),
          (SourcedSprite.Transform.Scale * SourcedSprite.SourceSprite.Region.Size.ToVector2()).ToNumerics(), 
          SourcedSprite.Origin.ToNumerics(), 
          SourcedSprite.Transform.Rotation, 
          SourcedSprite.Color.ToImColor()); 

      if (ImGui.IsWindowHovered())
      {
        if (_shape != null) 
        {
          HandleInput();
          _shape.Render(ImGui.GetWindowDrawList(), Color.Blue);
        }
        HandleMoveZoom();
      }

      foreach (var shapeModel in _shapeModels)
      {
        ImGui.SameLine();
        var shapeInstance = shapeModel;
        var icon = shapeModel.Icon;

        // pressed; begin annotation
        if (ImGui.Button(icon)) 
        {
          Mouse.SetCursor(MouseCursor.Crosshair);
          _shape = shapeInstance;
        }
      }   

    }
    Vector2 _initialMouse = Vector2.Zero;

    void HandleInput()
    {
      if (Nez.Input.LeftMouseButtonPressed)
      {
        _initialMouse = ImGui.GetMousePos();
        if (_shape is PointModel shape)
        {
          shape.Bounds = new RectangleF(_initialMouse, Vector2.Zero);
          Finish();
          return;
        }
      }
      if (Nez.Input.LeftMouseButtonDown)
      {
        // calculate position of area between mous drag
        var rect = new RectangleF();
        rect.Location = _initialMouse;
        rect.Size = ImGui.GetMousePos() - _initialMouse; 
        _shape.Bounds = rect;
 
      }
    }
    // Adds to current context
    void Finish()
    {
      Mouse.SetCursor(MouseCursor.Arrow);
      _propertied.Properties.Add(_shape);
      _initialMouse = Vector2.Zero;
      _shape = null;
    }
    void HandleMoveZoom()
    {
      var input = Core.GetGlobalManager<InputManager>();
      var mouse = ImGui.GetMousePos();
      mouse = ToWindow(mouse.ToVector2()).ToNumerics();

      // zooms
      if (ImGui.GetIO().MouseWheel != 0)
      {
        float zoomFactor = 1.2f;
        if (ImGui.GetIO().MouseWheel < 0) 
        {
          if (Zoom > 0.01) zoomFactor = 1/zoomFactor;
          else zoomFactor = 0.01f;
        }
        var zoom = Math.Clamp(Zoom * zoomFactor, 0.01f, 10f);
        var delta = ((Position - mouse)) * (zoomFactor - 1);
        if (zoomFactor != 1f) Position += delta;
        Zoom = zoom;
      }
      if (input.IsDragFirst)
      {
        _initialPosition = Position;
      }
      if (input.IsDrag && input.MouseDragButton == 2) 
      {
        Position = _initialPosition - (input.MouseDragStart - ImGui.GetMousePos()) / Zoom;        
      } 
    }
    Vector2 ToWindow(Vector2 screen) => new Vector2(screen.X-Bounds.X, screen.Y-Bounds.Y);
  }
}
