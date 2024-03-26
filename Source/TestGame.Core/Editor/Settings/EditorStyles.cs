using ImGuiNET;
using System.Numerics;

namespace Raven.Sheet
{
  public static partial class GuiStyles
  {
    public static void StyleViridescent()
    {
      var style = ImGui.GetStyle();
      style.WindowPadding = new Vector2(10, 6);
      style.FramePadding = new Vector2(12, 3);
      style.ItemSpacing = new Vector2(9, 6);
      style.ItemInnerSpacing = new Vector2(5, 4);
      style.TouchExtraPadding = new Vector2(0, 0);
      style.IndentSpacing = 22;
      style.ScrollbarSize = 11;
      style.GrabMinSize = 11;
      style.WindowRounding = 0;
      style.WindowTitleAlign = new Vector2(0.5f, 0.5f);
      style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
      style.SeparatorTextAlign = new Vector2(0.5f, 0.5f);
      style.SeparatorTextPadding = new Vector2(0f, 16);
      style.SeparatorTextBorderSize = 1;
      style.WindowBorderSize = 1;
      style.PopupBorderSize = 1;
      style.FrameBorderSize = 0;
      style.ChildBorderSize = 1;
      style.TabBorderSize = 0;
      style.WindowMenuButtonPosition = ImGuiDir.None;

      style.Colors[(int) ImGuiCol.ChildBg] = new Vector4(0.083f, 0.084f, 0.097f, 1f);
      style.Colors[(int) ImGuiCol.Border] = new Vector4(0.1f, 0.1f, 0.1f, 1f);
      style.Colors[(int) ImGuiCol.Text] = new Vector4(0.837f, 0.901f, 0.816f, 0.840f);
      style.Colors[(int) ImGuiCol.TextDisabled] = new Vector4(0.6f, 0.6f, 0.6f, 1f);
      style.Colors[(int) ImGuiCol.WindowBg] = new Vector4(0.063f, 0.064f, 0.077f, 1f);
      style.Colors[(int) ImGuiCol.PopupBg] = new Vector4(0.0f, 0.0f, 0.0f, 1f);
      style.Colors[(int) ImGuiCol.FrameBg] = new Vector4(0.184f, 0.21f, 0.206f, 1f);
      style.Colors[(int) ImGuiCol.TitleBg] = new Vector4(0.0f, 0.0f, 0.0f, 1f);
      style.Colors[(int) ImGuiCol.TitleBgActive] = new Vector4(0.0f, 0.0f, 0.0f, 1f);
      style.Colors[(int) ImGuiCol.MenuBarBg] = new Vector4(0.01f, 0.01f, 0.02f, 1f);
      style.Colors[(int) ImGuiCol.MenuBarBg] = new Vector4(0.01f, 0.01f, 0.02f, 1f);
      style.Colors[(int) ImGuiCol.ScrollbarBg] = new Vector4(0.184f, 0.21f, 0.206f, 1f);
      style.Colors[(int) ImGuiCol.CheckMark] = new Vector4(0.362f, 0.754f, 0.564f, 1f);
      style.Colors[(int) ImGuiCol.Button] = new Vector4(0.224f, 0.243f, 0.230f, 0.5f);
      style.Colors[(int) ImGuiCol.ButtonHovered] = new Vector4(0.472f, 0.687f, 0.477f, 1f);
      style.Colors[(int) ImGuiCol.Header] = new Vector4(0.172f, 0.218f, 0.19f, 0.530f);
      style.Colors[(int) ImGuiCol.HeaderHovered] = new Vector4(0.472f, 0.687f, 0.477f, 1f);
      style.Colors[(int) ImGuiCol.HeaderActive] = new Vector4(0.380f, 0.830f, 0.644f, 1f);
      style.Colors[(int) ImGuiCol.ResizeGrip] = new Vector4(0.0f, 0.0f, 0.0f, 1f);
      style.Colors[(int) ImGuiCol.Tab] = new Vector4(0.141f, 0.218f, 0.211f, 1f);
      style.Colors[(int) ImGuiCol.TabHovered] = new Vector4(0.330f, 0.829f, 0.307f, 0.68f);
      style.Colors[(int) ImGuiCol.TabActive] = new Vector4(0.063f, 0.064f, 0.077f, 1f);
      style.Colors[(int) ImGuiCol.TabUnfocusedActive] = new Vector4(0.141f, 0.218f, 0.211f, 1f);
    }
    public static void StyleViolet()
    {
      var style = ImGui.GetStyle();
      style.WindowPadding = new Vector2(10, 6);
      style.FramePadding = new Vector2(6, 3);
      style.ItemSpacing = new Vector2(6, 6);
      style.ItemInnerSpacing = new Vector2(4, 4);
      style.TouchExtraPadding = new Vector2(0, 0);
      style.IndentSpacing = 15;
      style.ScrollbarSize = 11;
      style.GrabMinSize = 11;
      style.WindowRounding = 0;
      style.WindowTitleAlign = new Vector2(0.5f, 0.5f);
      style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
      style.SeparatorTextAlign = new Vector2(0.5f, 0.5f);
      style.SeparatorTextPadding = new Vector2(0f, 16);
      style.SeparatorTextBorderSize = 1;
      style.WindowBorderSize = 0;
      style.PopupBorderSize = 0;
      style.FrameBorderSize = 0;
      style.ChildBorderSize = 1;
      style.TabBorderSize = 0;
      style.WindowMenuButtonPosition = ImGuiDir.None;

      style.Colors[(int) ImGuiCol.ChildBg] = new Vector4(0.083f, 0.084f, 0.097f, 1f);
      style.Colors[(int) ImGuiCol.Border] = new Vector4(0.497f, 0.327f, 0.526f, 1f);
      style.Colors[(int) ImGuiCol.Text] = new Vector4(0.837f, 0.901f, 0.816f, 0.900f);
      style.Colors[(int) ImGuiCol.TextDisabled] = new Vector4(0.6f, 0.6f, 0.6f, 1f);
      style.Colors[(int) ImGuiCol.WindowBg] = new Vector4(0.063f, 0.064f, 0.077f, 1f);
      style.Colors[(int) ImGuiCol.PopupBg] = new Vector4(0.0f, 0.0f, 0.0f, 1f);
      style.Colors[(int) ImGuiCol.FrameBg] = new Vector4(0.205f, 0.184f, 0.210f, 1f);
      style.Colors[(int) ImGuiCol.TitleBg] = new Vector4(0.0f, 0.0f, 0.0f, 1f);
      style.Colors[(int) ImGuiCol.TitleBgActive] = new Vector4(0.0f, 0.0f, 0.0f, 1f);
      style.Colors[(int) ImGuiCol.MenuBarBg] = new Vector4(0.01f, 0.01f, 0.02f, 1f);

      style.Colors[(int) ImGuiCol.ScrollbarBg] = new Vector4(0.0f, 0.0f, 0.0f, 1f);
      style.Colors[(int) ImGuiCol.ScrollbarGrab] = new Vector4(0.497f, 0.327f, 0.526f, 0.5f);
      style.Colors[(int) ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.497f, 0.327f, 0.526f, 1f);
      style.Colors[(int) ImGuiCol.ScrollbarGrabActive] = new Vector4(0.380f, 0.830f, 0.644f, 1f);

      style.Colors[(int) ImGuiCol.CheckMark] = new Vector4(0.650f, 0.326f, 0.754f, 1f);
      style.Colors[(int) ImGuiCol.Button] = new Vector4(0.224f, 0.243f, 0.230f, 0.5f);
      style.Colors[(int) ImGuiCol.ButtonHovered] = new Vector4(0.650f, 0.326f, 0.754f, 1f);
      style.Colors[(int) ImGuiCol.ButtonActive] = new Vector4(0.380f, 0.830f, 0.644f, 1f);

      style.Colors[(int) ImGuiCol.Header] = new Vector4(0.212f, 0.172f, 0.218f, 0.530f);
      style.Colors[(int) ImGuiCol.HeaderHovered] = new Vector4(0.361f, 0.186f, 0.512f, 1f);
      style.Colors[(int) ImGuiCol.HeaderActive] = new Vector4(0.380f, 0.830f, 0.644f, 1f);

      style.Colors[(int) ImGuiCol.Separator] = new Vector4(0.497f, 0.327f, 0.526f, 1f);
      style.Colors[(int) ImGuiCol.SeparatorHovered] = new Vector4(0.597f, 0.427f, 0.526f, 1f);
      style.Colors[(int) ImGuiCol.SeparatorActive] = new Vector4(0.380f, 0.830f, 0.644f, 1f);


      style.Colors[(int) ImGuiCol.ResizeGrip] = new Vector4(0.0f, 0.0f, 0.0f, 1f);
      style.Colors[(int) ImGuiCol.Tab] = new Vector4(0.212f, 0.172f, 0.218f, 0.530f);
      style.Colors[(int) ImGuiCol.TabHovered] = new Vector4(0.361f, 0.186f, 0.512f, 1f);
      style.Colors[(int) ImGuiCol.TabActive] = new Vector4(0.063f, 0.064f, 0.077f, 1f);
      style.Colors[(int) ImGuiCol.TabUnfocusedActive] = new Vector4(0.0f, 0.0f, 0.0f, 1f);
    }
  }
}
