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
    public ISceneSprite SourcedSprite;
    Vector2 _initialPosition = Vector2.Zero;
    IPropertied _propertied;
    ShapeModel _shape;
    readonly EditorColors _colors;
    bool _isStartEdit = false;
    Action<ShapeModel> _onFinish;

    public AnnotatorPane(EditorColors colors) 
    {
      _isOpen = false;
      _colors = colors;
      NoClose = false;

    } 

    public void Edit(ISceneSprite sprite, IPropertied propertied, Action<ShapeModel> onFinish=null)
    {
      _propertied = propertied;
      SourcedSprite = sprite;
      _isOpen = true;
      _isStartEdit = true;
      _onFinish = onFinish;
    }
    public void UnEdit()
    {
      _isOpen = false;
    }

    public override void OnRender(ImGuiWinManager imgui)
    {
      if (_isStartEdit)
      {
        var max1 = Math.Max(SourcedSprite.SourceSprite.Region.Size.X, SourcedSprite.SourceSprite.Region.Size.Y);
        var max2 = Math.Max(ImGui.GetWindowWidth(), ImGui.GetWindowHeight());
        Zoom = max2 / max1;
        Position = Vector2.Zero;
        // Position += ImGui.GetWindowSize() / 2 - (SourcedSprite.SourceSprite.Region.Size.ToVector2() * Zoom);
        // Position.Y += SourcedSprite.SourceSprite.Region.Size.Y * Zoom;
        _startZoom = Zoom;
        _isStartEdit = false;
      }
      var sourcePosition = (Position + ImGui.GetCursorScreenPos()).ToNumerics();
      var SourceSize = (SourcedSprite.Transform.Scale * Zoom * SourcedSprite.SourceSprite.Region.Size.ToVector2()).ToNumerics();
      ImGui.GetWindowDrawList().AddRectFilled(sourcePosition, sourcePosition + SourceSize, _colors.SheetSheet.ToImColor());
      // ImGui.GetWindowDrawList().AddRectFilled(SourcedSprite)
      ImGuiUtils.DrawImage(ImGui.GetWindowDrawList(), SourcedSprite.SourceSprite.Texture, 
          SourcedSprite.SourceSprite.Region, 
          sourcePosition,
          SourceSize, 
          SourcedSprite.Origin.ToNumerics(), 
          SourcedSprite.Transform.Rotation, 
          SourcedSprite.Color.ToImColor()); 

      if (_propertied != null)
      {
        foreach (var prop in _propertied.Properties)
        {
          if (prop.Value is ShapeModel shape) 
          {
            shape.Render(ImGui.GetWindowDrawList(), sourcePosition, Zoom, _colors.ShapeInactive.ToColor(), _colors.ShapeOutlineActive.ToColor());
          }
        }
      }
      if (ImGui.IsWindowHovered())
      {
        if (_shape != null) 
        {
          if (HandleInput()) 
          {
            Finish();
            return;
          }
          _shape.Render(ImGui.GetWindowDrawList(), _colors.ShapeActive.ToColor(), _colors.ShapeOutlineActive.ToColor());
        }
        HandleMoveZoom();
      }

      foreach (var shapeModel in _shapeModels)
      {
        ImGui.SameLine();
        var shapeInstance = shapeModel.Duplicate() as ShapeModel;
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
    float _startZoom = 1;

    bool HandleInput()
    {
      if (Nez.Input.LeftMouseButtonPressed)
      {
        _initialMouse = ImGui.GetMousePos();
        if (_shape is PointModel shape)
        {
          shape.Bounds = new RectangleF(_initialMouse, Vector2.Zero);
          return true;
        }
        else if (_shape is PolygonModel poly)
        {
          poly.Points.Add(_initialMouse);
          if (poly.Points.Count() >= 3 && Collisions.CircleToPoint(poly.Points[0], 10, _initialMouse))
            return true;
        }
      }
      if (Nez.Input.LeftMouseButtonDown && _shape is not PolygonModel)
      {
        // calculate position of area between mous drag
        var rect = new RectangleF();
        rect.Location = _initialMouse;
        rect.Size = ImGui.GetMousePos() - _initialMouse; 
        _shape.Bounds = rect; 
      }
      if (Nez.Input.LeftMouseButtonReleased && _shape is not PolygonModel)
      {
        return true;
      }
      return false;
    }
    // Adds to current context
    void Finish()
    {
      Mouse.SetCursor(MouseCursor.Arrow);
      if (_shape is PolygonModel poly) 
      {
        for (int i = 0; i < poly.Points.Count(); i++)
        {
          poly.Points[i] = poly.Points[i] - ImGui.GetCursorScreenPos() - Position;
          poly.Points[i] /= Zoom;
        }

      }
      else 
      {
        var bounds = _shape.Bounds;
        var screenMouse = _initialMouse - ImGui.GetCursorScreenPos();
        bounds.Location = screenMouse - Position;
        // bounds.Location -= SourcedSprite.Transform.Position;
        bounds.Location /= Zoom;
        bounds.Size /= Zoom;
        _shape.Bounds = bounds;
      }
      if (_onFinish != null) _onFinish(_shape);
      _propertied.Properties.Add(_shape);
      _initialMouse = Vector2.Zero;
      _shape = null;
    }
    Vector2 ToSourcePoint(Vector2 point)
    {
      var screenMouse = _initialMouse - ImGui.GetCursorScreenPos();
      point = screenMouse - Position;
      point /= Zoom;
      return point;
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
        Position = _initialPosition - (input.MouseDragStart - ImGui.GetMousePos()) ;        
      } 
    }
    Vector2 ToWindow(Vector2 screen) => new Vector2(screen.X-Bounds.X, screen.Y-Bounds.Y);
  }
}
