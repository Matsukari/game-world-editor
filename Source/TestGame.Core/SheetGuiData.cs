using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.ImGuiTools;
using Num = System.Numerics;


namespace Tools 
{
  // public enum ShapeType { Circle, Rectangle, Ellipse, Point, Polygon, None };
  public class Shape : ProppedObject
  {
    public string Name { get; set; } = "";
    public CustomProperties Properties { get; set; } = new CustomProperties();
    public RectangleF Bounds { get; set; } = new RectangleF();
    public Shape() {}
    public class Circle : Shape
    {  
    }
    public class Ellipse : Shape
    {  
    }
    public class Polygon : Shape
    {  
    }
    public class Rectangle : Shape
    {  
    }
    public class Point : Shape
    {  
    }
  }

  public partial class SpriteSheetEditor 
  {
    public class GuiData 
    {
      public IntPtr SheetTexture;
      public Num.Vector2 SheetPosition = new Num.Vector2();
      public Object Selection = null; 
      public float ContentZoom = 1;
      public bool IsDrag = false;
      public bool IsDragFirst = false;
      public bool IsDragLast = false;
      public Shape ShapeSelection = null;
      public ProppedObject ShapeContext = null;
      public int MouseDragButton = -1; 
      public RectangleF MouseDragArea = new RectangleF();
      public Num.Vector2 MouseDragStart = new Num.Vector2();
      public SelectionRectangle SelectionRect = null;
      public ImFontPtr NormalFont;
      public GuiData() 
      {
        SelectionRect = new SelectionRectangle(new RectangleF(), this);
        NormalFont = ImGui.GetIO().Fonts.AddFontFromFileTTF("RobotoCondensed-Regular.ttf", 16);
      }
      public Texture2D LoadTexture(string filename)
      {
        var texture = Texture2D.FromStream(Core.GraphicsDevice, File.OpenRead(filename));
        SheetTexture = texture.BindImGui();
        return texture;
      }
      public void LoadTextureFromFilePopup()
      {
        var isOpen = true;
        if (ImGui.BeginPopupModal(Names.OpenFile, ref isOpen, ImGuiWindowFlags.NoTitleBar))
        {
          var picker = FilePicker.GetFilePicker(this, Path.Combine(Environment.CurrentDirectory, "Content"), ".png|.atlas");
          picker.DontAllowTraverselBeyondRootFolder = true;
          if (picker.Draw())
          {
            LoadTexture(picker.SelectedFile);
            FilePicker.RemoveFilePicker(this);
          }
          ImGui.EndPopup();
        }
      }
    }
  }
}
