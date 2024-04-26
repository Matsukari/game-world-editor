using Microsoft.Xna.Framework;
using ImGuiNET;

namespace Raven
{
  public class EditorColors
  {
    // Important
    public Vector4 Accent = new Vector4(0.561f, 0.386f, 0.712f, 1f); 
    public Vector4 Background = new Vector4(0.15f, 0.15f, 0.15f, 1f);

    public Vector4 ToolbarBg = new Vector4(0.561f, 0.386f, 0.712f, 1f); 

    // Animation colors
    public uint FrameInactive { get => Get(ImGuiCol.TextDisabled); }
    public uint FrameActive { get => Get(ImGuiCol.Text); }

    public Vector4 OriginPoint = new Vector4(0.5f, 0.5f, 0.5f, 1f);
    public Vector4 SpriteBoundsOutline = new Vector4(0.5f, 0.5f, 0.5f, 1f);

    // Guidelines
    public Vector4 OriginLineX = new Vector4(0.5f, 0.5f, 0.5f, 1f);
    public Vector4 OriginLineY = new Vector4(0.5f, 0.5f, 0.5f, 1f);
    public Vector4 LevelGrid = new Vector4(0.0f, 0.0f, 0.0f, 0.5f);
    public Vector4 LevelSelOutline = new Vector4(0.85f, 0.85f, 0.85f, 1f);    

    // Sprite picker
    public Vector4 PickHoverOutline = new Vector4(0.5f, 0.5f, 0.5f, 1f);
    public Vector4 PickSelectedOutline = new Vector4(0.7f, 0.7f, 0.7f, 1f);
    public Vector4 PickHover = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
    public Vector4 PickFill = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);

    // Selection
    public Vector4 SelectionOutline = new Vector4(0.85f, 0.85f, 0.85f, 1f);
    public Vector4 SelectionFill = new Vector4(0.2f, 0.2f, 0.2f, 0.04f);
    public Vector4 SelectionPoint = new Vector4(0.9f, 0.9f, 0.9f, 1f);

    // Shapes
    public Vector4 ShapeActive = new Vector4(0.5f, 0.5f, 0.5f, 0.2f);
    public Vector4 ShapeOutlineActive = new Vector4(0.5f, 0.5f, 0.7f, 1f);
    public Vector4 ShapeInactive = new Vector4(0.3f, 0.3f, 0.3f, 0.1f);
    public Vector4 ShapeName = new Vector4(0.5f, 0.5f, 0.7f, 1f);

    // Pads
    public Vector4 LevelSheet = new Vector4(0.20f, 0.2f, 0.2f, 2f);
    public Vector4 SheetSheet = new Vector4(0.20f, 0.2f, 0.2f, 2f);

    public static uint Get(ImGuiCol col) => ImGui.ColorConvertFloat4ToU32(ImGui.GetStyle().Colors[(int) col]);

  }
}
