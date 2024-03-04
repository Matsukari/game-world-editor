using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.ImGuiTools;
using Num = System.Numerics;


namespace Raven.Sheet
{
  public static class WorldSpaceExt
  {

  }
  public class GuiData 
  {
    public Num.Vector2 SheetPosition = new Num.Vector2();
    public Texture2D SheetTexture;
    public Object Selection = null; 
    public float ContentZoom = 1;

    public bool IsDrag = false;
    public bool IsDragFirst = false;
    public bool IsDragLast = false;

    public Shape ShapeSelection = null;
    public IPropertied ShapeContext = null;

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
