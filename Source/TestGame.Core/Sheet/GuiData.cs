using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.ImGuiTools;
using Num = System.Numerics;


namespace Raven.Sheet
{
  public class GuiData 
  {
    public Num.Vector2 SheetPosition = new Num.Vector2();
    public Texture2D SheetTexture;
    public Object Selection = null; 
    public float Zoom = 1;

    public Shape ShapeSelection = null;
    public IPropertied ShapeContext = null;

    public Selection SelectionRect = null;
    public ImFontPtr NormalFont;

    public GuiData() 
    {
      SelectionRect = new Selection(this);
      // NormalFont = ImGui.GetIO().Fonts.AddFontFromFileTTF("RobotoCondensed-Regular.ttf", 16);
      // float baseFontSize = 13.0f; // 13.0f is the size of the default font. Change to the font size you use.
      // float iconFontSize = baseFontSize * 2.0f / 3.0f; // FontAwesome fonts need to have their sizes reduced by 2.0f/3.0f in order to align correctly
      // var icons_ranges = new ushort[]{ IconFonts.FontAwesome5.IconMin, IconFonts.FontAwesome5.IconMax, 0 };
      // var font = new Tuple<string, int>("", 12);
      // unsafe 
      // {   
      //   ImFontConfigPtr icons_config = ImGuiNative.ImFontConfig_ImFontConfig();
      //   icons_config.MergeMode = true;
      //   icons_config.PixelSnapH = true; 
      //   icons_config.GlyphMinAdvanceX = iconFontSize;
      //   fixed (ushort* icons_ranges_ptr = &icons_ranges[0])
      //   {
      //     ImGui.GetIO().Fonts.AddFontFromFileTTF(font.Item1, iconFontSize, icons_config, (IntPtr)icons_ranges_ptr);
      //   }
      // } 
    }
    public Texture2D LoadTexture(string filename)
    {
      var texture = Texture2D.FromStream(Core.GraphicsDevice, File.OpenRead(filename));
      SheetTexture = texture;
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
