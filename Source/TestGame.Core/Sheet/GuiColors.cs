using Microsoft.Xna.Framework;
using ImGuiNET;

namespace Raven.Sheet
{
  public class GuiColors 
  {
    public Color SelectionOutline = new Color(0.85f, 0.85f, 0.85f);
    public Color SelectionFill = new Color(0.2f, 0.2f, 0.2f, 0.04f);
    public Color SpriteGridOutline = new Color(0.3f, 0.3f, 0.3f, 0.17f);
    public Color SpriteRegionInactiveOutline = new Color(0.0f, 0.0f, 0.0f, 0.5f);
    public Color SheetInactiveFill = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    public Color SpriteRegionActiveOutline = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color SpriteRegionActiveFill = new Color(0.2f, 0.2f, 0.2f, 0.1f);
    public Color SelectionPoint = new Color(0.9f, 0.9f, 0.9f);
    public Color ContentActiveOutline = Color.CadetBlue;
    public Color AnnotatedShapeActive = new Color(0.5f, 0.5f, 0.5f, 0.2f);
    public Color AnnotatedShapeOutlineActive = new Color(0.5f, 0.5f, 0.7f, 1f);
    public Color AnnotatedShapeInactive = new Color(0.3f, 0.3f, 0.3f, 0.1f);
    public Color AnnotatedName = new Color(0.5f, 0.5f, 0.7f, 1f);
    public Color Background = new Color(0.15f, 0.15f, 0.15f);
    public Color DeleteButton = Color.PaleVioletRed;

    public Color PaintTilePreviewColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);

    public Color ViewbarSpecialButton = Color.Turquoise;
    public Color ViewbarViewButton = Color.PaleVioletRed;
    public Color ViewbarShapeButton = Color.OrangeRed;

    public static uint Get(ImGuiCol col) => ImGui.ColorConvertFloat4ToU32(ImGui.GetStyle().Colors[(int) col]);


    public Color LevelSheet = new Color(0.20f, 0.2f, 0.2f, 2f);


    public GuiColors() {}
  }
}
